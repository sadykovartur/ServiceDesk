import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Alert,
  Button,
  Descriptions,
  Form,
  Input,
  message,
  Modal,
  Select,
  Space,
  Typography,
} from 'antd';
import { ApiError } from '../api/client';
import { ticketsApi } from '../api/tickets';
import type { TicketResponse, TicketStatus } from '../api/tickets';
import { useAuth } from '../contexts/AuthContext';
import PageLoading from '../components/PageLoading';
import PageNotFound from '../components/PageNotFound';
import PageError from '../components/PageError';
import TicketStatusTag from '../components/TicketStatusTag';
import TicketPriorityTag from '../components/TicketPriorityTag';

const { Title } = Typography;

const DATE_FORMAT: Intl.DateTimeFormatOptions = {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
};

function formatDate(iso: string) {
  return new Date(iso).toLocaleString('en-GB', DATE_FORMAT);
}

const NEXT_STATUSES: Partial<Record<TicketStatus, { label: string; value: TicketStatus }[]>> = {
  InProgress: [
    { label: 'Waiting for Student', value: 'WaitingForStudent' },
    { label: 'Resolved', value: 'Resolved' },
  ],
  WaitingForStudent: [
    { label: 'In Progress', value: 'InProgress' },
    { label: 'Resolved', value: 'Resolved' },
  ],
};

interface OperatorActionsProps {
  ticket: TicketResponse;
  ticketQueryKey: string;
}

function OperatorActions({ ticket, ticketQueryKey }: OperatorActionsProps) {
  const { user, role } = useAuth();
  const queryClient = useQueryClient();
  const [rejectOpen, setRejectOpen] = useState(false);
  const [rejectReason, setRejectReason] = useState('');
  const [reasonError, setReasonError] = useState(false);

  const assignMutation = useMutation({
    mutationFn: () => ticketsApi.assignToMe(ticket.id),
    onSuccess: () => {
      void message.success('Ticket assigned to you');
      invalidate();
    },
    onError: (err) => {
      if (err instanceof ApiError && err.status === 409) {
        void message.error('Ticket was assigned to another operator');
      } else {
        void message.error(err instanceof ApiError ? err.message : 'Failed to assign ticket');
      }
    },
  });

  const statusMutation = useMutation({
    mutationFn: (status: TicketStatus) => ticketsApi.changeStatus(ticket.id, status),
    onSuccess: () => {
      void message.success('Status updated');
      invalidate();
    },
    onError: (err) => {
      if (err instanceof ApiError && err.status === 409) {
        void message.error('Ticket was assigned to another operator');
      } else {
        void message.error(err instanceof ApiError ? err.message : 'Failed to update status');
      }
    },
  });

  const rejectMutation = useMutation({
    mutationFn: (reason: string) => ticketsApi.rejectTicket(ticket.id, reason),
    onSuccess: () => {
      void message.success('Ticket rejected');
      setRejectOpen(false);
      setRejectReason('');
      setReasonError(false);
      invalidate();
    },
    onError: (err) => {
      if (err instanceof ApiError && err.status === 409) {
        void message.error('Ticket was assigned to another operator');
      } else {
        void message.error(err instanceof ApiError ? err.message : 'Failed to reject ticket');
      }
    },
  });

  if (role !== 'Operator') return null;

  const isAssignee = ticket.assignee?.id === user?.id;
  const isUnassigned = ticket.assignee == null;

  const canAssignToMe =
    ['New', 'InProgress', 'WaitingForStudent'].includes(ticket.status) &&
    (isUnassigned || isAssignee);

  const canChangeStatus = isAssignee && (ticket.status === 'InProgress' || ticket.status === 'WaitingForStudent');

  const canReject =
    !['Closed', 'Rejected', 'Resolved'].includes(ticket.status) && (isUnassigned || isAssignee);

  if (!canAssignToMe && !canChangeStatus && !canReject) return null;

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['ticket', ticketQueryKey] });
    queryClient.invalidateQueries({ queryKey: ['tickets'] });
  };  

  const handleRejectOk = () => {
    if (!rejectReason.trim()) {
      setReasonError(true);
      return;
    }
    rejectMutation.mutate(rejectReason.trim());
  };

  const handleRejectCancel = () => {
    setRejectOpen(false);
    setRejectReason('');
    setReasonError(false);
  };

  const nextStatusOptions = NEXT_STATUSES[ticket.status] ?? [];

  return (
    <>
      <Space style={{ marginBottom: 16 }} wrap>
        {canAssignToMe && (
          <Button
            type="primary"
            loading={assignMutation.isPending}
            onClick={() => assignMutation.mutate()}
          >
            Assign to me
          </Button>
        )}
        {canChangeStatus && (
          <Select
            placeholder="Change status…"
            style={{ width: 210 }}
            loading={statusMutation.isPending}
            disabled={statusMutation.isPending}
            value={null}
            options={nextStatusOptions}
            onChange={(val: TicketStatus) => statusMutation.mutate(val)}
          />
        )}
        {canReject && (
          <Button danger onClick={() => setRejectOpen(true)}>
            Reject
          </Button>
        )}
      </Space>

      <Modal
        title="Reject Ticket"
        open={rejectOpen}
        onCancel={handleRejectCancel}
        onOk={handleRejectOk}
        confirmLoading={rejectMutation.isPending}
        okText="Reject"
        okButtonProps={{ danger: true }}
        destroyOnHidden
      >
        <Form layout="vertical">
          <Form.Item
            label="Reason"
            required
            validateStatus={reasonError ? 'error' : undefined}
            help={reasonError ? 'Reason is required' : undefined}
          >
            <Input.TextArea
              rows={4}
              value={rejectReason}
              onChange={(e) => {
                setRejectReason(e.target.value);
                if (e.target.value.trim()) setReasonError(false);
              }}
              placeholder="Enter rejection reason…"
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}

export default function TicketDetailPage() {
  const { id } = useParams<{ id: string }>();

  const { data: ticket, isLoading, isError, error } = useQuery({
    queryKey: ['ticket', id],
    queryFn: () => ticketsApi.getById(id!),
    enabled: Boolean(id),
  });

  if (isLoading) return <PageLoading />;

  if (isError) {
    if (error instanceof ApiError && error.status === 404) return <PageNotFound />;
    return <PageError message={error instanceof ApiError ? error.message : undefined} />;
  }

  if (!ticket) return <PageNotFound />;

  return (
    <>
      <Title level={4} style={{ marginTop: 0 }}>
        {ticket.title}
      </Title>

      {ticket.status === 'Rejected' && ticket.rejectedReason && (
        <Alert
          type="error"
          message="Ticket Rejected"
          description={ticket.rejectedReason}
          style={{ marginBottom: 16 }}
        />
      )}

      <OperatorActions ticket={ticket} ticketQueryKey={id!}/>

      <Descriptions bordered column={1} style={{ marginBottom: 24 }}>
        <Descriptions.Item label="Status">
          <TicketStatusTag status={ticket.status} />
        </Descriptions.Item>
        <Descriptions.Item label="Priority">
          <TicketPriorityTag priority={ticket.priority} />
        </Descriptions.Item>
        <Descriptions.Item label="Category">{ticket.category.name}</Descriptions.Item>
        <Descriptions.Item label="Author">{ticket.author.displayName}</Descriptions.Item>
        <Descriptions.Item label="Assignee">
          {ticket.assignee?.displayName ?? 'Unassigned'}
        </Descriptions.Item>
        <Descriptions.Item label="Created">{formatDate(ticket.createdAt)}</Descriptions.Item>
        <Descriptions.Item label="Updated">{formatDate(ticket.updatedAt)}</Descriptions.Item>
        <Descriptions.Item label="Description">
          <span style={{ whiteSpace: 'pre-wrap' }}>{ticket.description}</span>
        </Descriptions.Item>
      </Descriptions>
    </>
  );
}

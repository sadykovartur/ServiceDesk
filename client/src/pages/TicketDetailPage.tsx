import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Alert, Descriptions, Typography } from 'antd';
import { ApiError } from '../api/client';
import { ticketsApi } from '../api/tickets';
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

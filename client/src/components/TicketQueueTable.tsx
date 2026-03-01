import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Select, Space, Table, Typography } from 'antd';
import type { TableColumnsType } from 'antd';
import { useNavigate } from 'react-router-dom';
import { ApiError } from '../api/client';
import { ticketsApi } from '../api/tickets';
import type { TicketResponse, TicketStatus, TicketPriority } from '../api/tickets';
import PageLoading from './PageLoading';
import PageEmpty from './PageEmpty';
import PageError from './PageError';
import TicketStatusTag from './TicketStatusTag';
import TicketPriorityTag from './TicketPriorityTag';

const { Title } = Typography;

const STATUS_OPTIONS: { label: string; value: TicketStatus }[] = [
  { label: 'New', value: 'New' },
  { label: 'In Progress', value: 'InProgress' },
  { label: 'Waiting for Student', value: 'WaitingForStudent' },
  { label: 'Resolved', value: 'Resolved' },
  { label: 'Closed', value: 'Closed' },
  { label: 'Rejected', value: 'Rejected' },
];

const PRIORITY_OPTIONS: { label: string; value: TicketPriority }[] = [
  { label: 'Low', value: 'Low' },
  { label: 'Medium', value: 'Medium' },
  { label: 'High', value: 'High' },
];

const DATE_FORMAT: Intl.DateTimeFormatOptions = {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
};

const COLUMNS: TableColumnsType<TicketResponse> = [
  { title: 'Title', dataIndex: 'title', key: 'title' },
  { title: 'Category', dataIndex: ['category', 'name'], key: 'category' },
  {
    title: 'Priority',
    dataIndex: 'priority',
    key: 'priority',
    width: 110,
    render: (priority: TicketPriority) => <TicketPriorityTag priority={priority} />,
  },
  {
    title: 'Status',
    dataIndex: 'status',
    key: 'status',
    width: 190,
    render: (status: TicketStatus) => <TicketStatusTag status={status} />,
  },
  {
    title: 'Created',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 160,
    render: (v: string) => new Date(v).toLocaleString('en-GB', DATE_FORMAT),
  },
];

interface Props {
  title: string;
  defaultStatus?: TicketStatus;
  fixedAssignedToMe?: boolean;
}

export default function TicketQueueTable({ title, defaultStatus, fixedAssignedToMe }: Props) {
  const navigate = useNavigate();
  const [status, setStatus] = useState<TicketStatus | undefined>(defaultStatus);
  const [priority, setPriority] = useState<TicketPriority | undefined>(undefined);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  const queryParams = {
    status,
    priority,
    assignedToMe: fixedAssignedToMe || undefined,
    page,
    pageSize,
  };

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['tickets', queryParams],
    queryFn: () => ticketsApi.list(queryParams),
  });

  if (isLoading) return <PageLoading />;
  if (isError)
    return <PageError message={error instanceof ApiError ? error.message : undefined} />;

  const tickets = data?.items ?? [];
  const total = data?.total ?? 0;

  return (
    <>
      <Title level={4} style={{ marginTop: 0 }}>
        {title}
      </Title>

      <Space style={{ marginBottom: 16 }} wrap>
        <Select
          allowClear
          placeholder="Filter by Status"
          style={{ width: 210 }}
          options={STATUS_OPTIONS}
          value={status}
          onChange={(val) => {
            setStatus(val);
            setPage(1);
          }}
        />
        <Select
          allowClear
          placeholder="Filter by Priority"
          style={{ width: 180 }}
          options={PRIORITY_OPTIONS}
          value={priority}
          onChange={(val) => {
            setPriority(val);
            setPage(1);
          }}
        />
      </Space>

      {tickets.length === 0 ? (
        <PageEmpty description="No tickets found" />
      ) : (
        <Table<TicketResponse>
          dataSource={tickets}
          columns={COLUMNS}
          rowKey="id"
          pagination={{
            current: page,
            pageSize,
            total,
            showSizeChanger: true,
            pageSizeOptions: ['10', '20', '50'],
            onChange: (newPage, newPageSize) => {
              setPage(newPage);
              setPageSize(newPageSize);
            },
          }}
          onRow={(record) => ({
            onClick: () => navigate(`/tickets/${record.id}`),
            style: { cursor: 'pointer' },
          })}
        />
      )}
    </>
  );
}

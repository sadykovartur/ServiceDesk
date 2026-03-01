import { Tag } from 'antd';
import type { TicketStatus } from '../api/tickets';

const STATUS_CONFIG: Record<TicketStatus, { color: string; label: string }> = {
  New: { color: 'blue', label: 'New' },
  InProgress: { color: 'processing', label: 'In Progress' },
  WaitingForStudent: { color: 'orange', label: 'Waiting for Student' },
  Resolved: { color: 'green', label: 'Resolved' },
  Closed: { color: 'default', label: 'Closed' },
  Rejected: { color: 'red', label: 'Rejected' },
};

interface Props {
  status: TicketStatus;
}

export default function TicketStatusTag({ status }: Props) {
  const { color, label } = STATUS_CONFIG[status] ?? { color: 'default', label: status };
  return <Tag color={color}>{label}</Tag>;
}

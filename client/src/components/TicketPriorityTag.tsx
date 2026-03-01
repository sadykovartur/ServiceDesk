import { Tag } from 'antd';
import type { TicketPriority } from '../api/tickets';

const PRIORITY_CONFIG: Record<TicketPriority, { color: string; label: string }> = {
  Low: { color: 'default', label: 'Low' },
  Medium: { color: 'orange', label: 'Medium' },
  High: { color: 'red', label: 'High' },
};

interface Props {
  priority: TicketPriority;
}

export default function TicketPriorityTag({ priority }: Props) {
  const { color, label } = PRIORITY_CONFIG[priority] ?? { color: 'default', label: priority };
  return <Tag color={color}>{label}</Tag>;
}

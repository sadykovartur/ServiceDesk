import { apiClient } from './client';

export type TicketStatus =
  | 'New'
  | 'InProgress'
  | 'WaitingForStudent'
  | 'Resolved'
  | 'Closed'
  | 'Rejected';

export type TicketPriority = 'Low' | 'Medium' | 'High';

export interface UserRef {
  id: string;
  displayName: string;
}

export interface CategoryRef {
  id: number;
  name: string;
  isActive: boolean;
}

export interface TicketResponse {
  id: string;
  title: string;
  description: string;
  priority: TicketPriority;
  status: TicketStatus;
  createdAt: string;
  updatedAt: string;
  rejectedReason?: string;
  category: CategoryRef;
  author: UserRef;
  assignee?: UserRef;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface TicketListParams {
  status?: TicketStatus;
  priority?: TicketPriority;
  categoryId?: number;
  search?: string;
  assignedToMe?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  priority: TicketPriority;
  categoryId: number;
}

function buildQuery(params: object): string {
  const qs = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null && value !== '') {
      qs.set(key, String(value));
    }
  }
  const str = qs.toString();
  return str ? `?${str}` : '';
}

export const ticketsApi = {
  list: (params: TicketListParams = {}) =>
    apiClient.get<PagedResult<TicketResponse>>(`/api/tickets${buildQuery(params)}`),
  getById: (id: string) => apiClient.get<TicketResponse>(`/api/tickets/${id}`),
  create: (data: CreateTicketRequest) => apiClient.post<TicketResponse>('/api/tickets', data),
};

import { httpClient } from "./httpClient";

export type Ticket = {
  id: number;
  title: string;
  description: string;
  categoryId: number;
  categoryName: string;
  priority: string;
  status: string;
  authorId: string;
  assigneeId: string | null;
  rejectedReason: string | null;
  createdAt: string;
  updatedAt: string;
};

export async function getTickets(): Promise<Ticket[]> {
  return httpClient.get<Ticket[]>("/api/tickets");
}

export async function getTicketById(id: number): Promise<Ticket> {
  return httpClient.get<Ticket>(`/api/tickets/${id}`);
}

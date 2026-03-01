import { apiClient } from './client';

export interface Category {
  id: number;
  name: string;
  isActive: boolean;
}

export const categoriesApi = {
  list: (includeInactive = false) =>
    apiClient.get<Category[]>(
      `/api/categories${includeInactive ? '?includeInactive=true' : ''}`,
    ),
};

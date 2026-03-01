import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Select, Table, Typography, message } from 'antd';
import type { TableColumnsType } from 'antd';
import { ApiError, apiClient } from '../../api/client';
import PageEmpty from '../../components/PageEmpty';
import PageError from '../../components/PageError';
import PageLoading from '../../components/PageLoading';

interface UserDto {
  id: string;
  displayName: string;
  email: string;
  role: string;
}

const usersKey = ['users'] as const;

const ROLES = ['Student', 'Operator', 'Admin'] as const;

export default function UsersPage() {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();

  const [localRoles, setLocalRoles] = useState<Record<string, string>>({});

  const { data, isLoading, isError, error } = useQuery({
    queryKey: usersKey,
    queryFn: () => apiClient.get<UserDto[]>('/api/users'),
  });

  const roleMutation = useMutation({
    mutationFn: ({ id, role }: { id: string; role: string }) =>
      apiClient.put<UserDto>(`/api/users/${id}/role`, { role }),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: usersKey });
      messageApi.success('Role updated');
      setLocalRoles((prev) => {
        const next = { ...prev };
        delete next[variables.id];
        return next;
      });
    },
    onError: (err) => {
      if (err instanceof ApiError) messageApi.error(err.message);
    },
  });

  const columns: TableColumnsType<UserDto> = [
    {
      title: 'Display Name',
      dataIndex: 'displayName',
      key: 'displayName',
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: 'Role',
      key: 'role',
      width: 240,
      render: (_, record) => {
        const selectedRole = localRoles[record.id] ?? record.role;
        const isPendingRow =
          roleMutation.isPending && roleMutation.variables?.id === record.id;

        return (
          <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
            <Select
              value={selectedRole}
              options={ROLES.map((r) => ({ value: r, label: r }))}
              style={{ width: 120 }}
              disabled={isPendingRow}
              onChange={(val) =>
                setLocalRoles((prev) => ({ ...prev, [record.id]: val }))
              }
            />
            <Button
              type="primary"
              loading={isPendingRow}
              disabled={isPendingRow || selectedRole === record.role}
              onClick={() => roleMutation.mutate({ id: record.id, role: selectedRole })}
            >
              Save
            </Button>
          </div>
        );
      },
    },
  ];

  if (isLoading) return <PageLoading />;
  if (isError)
    return <PageError message={(error instanceof ApiError ? error.message : undefined)} />;
  if (!data || data.length === 0) return <PageEmpty description="No users found" />;

  return (
    <>
      {contextHolder}
      <Table<UserDto>
        dataSource={data}
        columns={columns}
        rowKey="id"
        pagination={false}
      />
      <Typography.Text type="secondary" style={{ marginTop: 12, display: 'block' }}>
        Роль вступает в силу после следующего логина
      </Typography.Text>
    </>
  );
}

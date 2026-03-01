import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Form, Input, Modal, Switch, Table, message } from 'antd';
import type { TableColumnsType } from 'antd';
import { ApiError, apiClient } from '../../api/client';
import PageEmpty from '../../components/PageEmpty';
import PageError from '../../components/PageError';
import PageLoading from '../../components/PageLoading';

interface Category {
  id: number;
  name: string;
  isActive: boolean;
}

const categoriesKey = ['categories', { includeInactive: true }] as const;

export default function CategoriesPage() {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();

  const [modalOpen, setModalOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<Category | null>(null);
  const [form] = Form.useForm<{ name: string }>();

  const { data, isLoading, isError, error } = useQuery({
    queryKey: categoriesKey,
    queryFn: () => apiClient.get<Category[]>('/api/categories?includeInactive=true'),
  });

  const createMutation = useMutation({
    mutationFn: (name: string) => apiClient.post<Category>('/api/categories', { name }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoriesKey });
      messageApi.success('Category created');
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err) => {
      if (err instanceof ApiError) messageApi.error(err.message);
    },
  });

  const editMutation = useMutation({
    mutationFn: ({ id, name }: { id: number; name: string }) =>
      apiClient.put<Category>(`/api/categories/${id}`, { name }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoriesKey });
      messageApi.success('Category updated');
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err) => {
      if (err instanceof ApiError) messageApi.error(err.message);
    },
  });

  const toggleMutation = useMutation({
    mutationFn: ({ id, isActive }: { id: number; isActive: boolean }) =>
      apiClient.patch<Category>(`/api/categories/${id}/active`, { isActive }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoriesKey });
    },
    onError: (err) => {
      if (err instanceof ApiError) messageApi.error(err.message);
    },
  });

  const anyMutationPending =
    createMutation.isPending || editMutation.isPending || toggleMutation.isPending;
  const modalMutationPending = createMutation.isPending || editMutation.isPending;

  const handleOpenCreate = () => {
    setEditTarget(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleOpenEdit = (category: Category) => {
    setEditTarget(category);
    form.setFieldsValue({ name: category.name });
    setModalOpen(true);
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      const name = values.name.trim();
      if (editTarget) {
        editMutation.mutate({ id: editTarget.id, name });
      } else {
        createMutation.mutate(name);
      }
    } catch {
      // form validation failed — Ant Design shows inline errors
    }
  };

  const handleModalCancel = () => {
    if (modalMutationPending) return;
    setModalOpen(false);
    form.resetFields();
  };

  const columns: TableColumnsType<Category> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Active',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive: boolean, record) => (
        <Switch
          checked={isActive}
          disabled={anyMutationPending}
          onChange={(checked) => toggleMutation.mutate({ id: record.id, isActive: checked })}
        />
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 100,
      render: (_, record) => (
        <Button onClick={() => handleOpenEdit(record)}>Edit</Button>
      ),
    },
  ];

  if (isLoading) return <PageLoading />;
  if (isError)
    return <PageError message={(error instanceof ApiError ? error.message : undefined)} />;

  return (
    <>
      {contextHolder}
      <div style={{ marginBottom: 16 }}>
        <Button type="primary" onClick={handleOpenCreate}>
          Create Category
        </Button>
      </div>

      {!data || data.length === 0 ? (
        <PageEmpty description="No categories yet" />
      ) : (
        <Table<Category>
          dataSource={data}
          columns={columns}
          rowKey="id"
          pagination={false}
        />
      )}

      <Modal
        title={editTarget ? 'Edit Category' : 'Create Category'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        okButtonProps={{ disabled: modalMutationPending, loading: modalMutationPending }}
        cancelButtonProps={{ disabled: modalMutationPending }}
        destroyOnClose
      >
        <Form form={form} layout="vertical">
          <Form.Item
            name="name"
            label="Name"
            rules={[
              { required: true, message: 'Name is required' },
              { max: 100, message: 'Name must be at most 100 characters' },
            ]}
          >
            <Input maxLength={100} />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}

import { useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Alert, Button, Form, Input, Select, Typography, message } from 'antd';
import { useNavigate } from 'react-router-dom';
import { ApiError } from '../../api/client';
import { categoriesApi } from '../../api/categories';
import { ticketsApi } from '../../api/tickets';
import type { CreateTicketRequest, TicketPriority } from '../../api/tickets';
import PageLoading from '../../components/PageLoading';

const { Title } = Typography;
const { TextArea } = Input;

const PRIORITY_OPTIONS: { label: string; value: TicketPriority }[] = [
  { label: 'Low', value: 'Low' },
  { label: 'Medium', value: 'Medium' },
  { label: 'High', value: 'High' },
];

export default function NewTicketPage() {
  const navigate = useNavigate();
  const [form] = Form.useForm<CreateTicketRequest>();
  const [messageApi, contextHolder] = message.useMessage();
  const [serverError, setServerError] = useState<string | null>(null);

  const categoriesQuery = useQuery({
    queryKey: ['categories', { includeInactive: false }],
    queryFn: () => categoriesApi.list(false),
  });

  const mutation = useMutation({
    mutationFn: (data: CreateTicketRequest) => ticketsApi.create(data),
    onSuccess: () => {
      messageApi.success('Ticket submitted successfully');
      navigate('/tickets');
    },
    onError: (err) => {
      if (err instanceof ApiError) {
        setServerError(err.message);
      }
    },
  });

  const handleFinish = (values: CreateTicketRequest) => {
    setServerError(null);
    mutation.mutate(values);
  };

  if (categoriesQuery.isLoading) return <PageLoading />;

  const categoryOptions = (categoriesQuery.data ?? []).map((c) => ({
    label: c.name,
    value: c.id,
  }));

  return (
    <>
      {contextHolder}

      <Title level={4} style={{ marginTop: 0 }}>
        New Ticket
      </Title>

      {serverError && (
        <Alert
          type="error"
          message={serverError}
          style={{ marginBottom: 16 }}
          closable
          onClose={() => setServerError(null)}
        />
      )}

      <Form
        form={form}
        layout="vertical"
        style={{ maxWidth: 640 }}
        onFinish={handleFinish}
        disabled={mutation.isPending}
      >
        <Form.Item
          name="title"
          label="Title"
          rules={[{ required: true, message: 'Title is required' }]}
        >
          <Input maxLength={200} />
        </Form.Item>

        <Form.Item
          name="description"
          label="Description"
          rules={[{ required: true, message: 'Description is required' }]}
        >
          <TextArea rows={5} maxLength={2000} showCount />
        </Form.Item>

        <Form.Item
          name="priority"
          label="Priority"
          rules={[{ required: true, message: 'Priority is required' }]}
        >
          <Select options={PRIORITY_OPTIONS} placeholder="Select priority" />
        </Form.Item>

        <Form.Item
          name="categoryId"
          label="Category"
          rules={[{ required: true, message: 'Category is required' }]}
        >
          <Select
            options={categoryOptions}
            placeholder="Select category"
            loading={categoriesQuery.isLoading}
            notFoundContent={
              categoriesQuery.isError ? 'Failed to load categories' : 'No categories available'
            }
          />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={mutation.isPending}>
            Submit
          </Button>
          <Button style={{ marginLeft: 8 }} onClick={() => navigate('/tickets')}>
            Cancel
          </Button>
        </Form.Item>
      </Form>
    </>
  );
}

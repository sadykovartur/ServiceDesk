import { Button, Card, Space, Typography } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import type { Role } from '../contexts/AuthContext';

const DEMO_USERS: { role: Role; redirect: string }[] = [
  { role: 'Student', redirect: '/tickets' },
  { role: 'Operator', redirect: '/queue/new' },
  { role: 'Admin', redirect: '/admin/categories' },
];

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleDemoLogin = (role: Role, redirect: string) => {
    login('dev-token', { id: 'dev', displayName: `${role} Demo` }, role);
    navigate(redirect);
  };

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        background: '#f0f2f5',
      }}
    >
      <Card style={{ width: 360 }}>
        <Typography.Title level={3} style={{ textAlign: 'center', marginBottom: 8 }}>
          Service Desk
        </Typography.Title>
        <Typography.Text
          type="secondary"
          style={{ display: 'block', textAlign: 'center', marginBottom: 24 }}
        >
          Demo — select a role to continue
        </Typography.Text>

        <Space direction="vertical" style={{ width: '100%' }} size="middle">
          {DEMO_USERS.map(({ role, redirect }) => (
            <Button
              key={role}
              type="primary"
              block
              onClick={() => handleDemoLogin(role, redirect)}
            >
              Login as {role}
            </Button>
          ))}
        </Space>
      </Card>
    </div>
  );
}

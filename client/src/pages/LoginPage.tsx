import { Button, Card, CardContent, Stack, TextField, Typography } from "@mui/material";

function LoginPage(): JSX.Element {
  return (
    <Stack alignItems="center" justifyContent="center" sx={{ minHeight: "100vh", p: 2 }}>
      <Card sx={{ width: 420, maxWidth: "100%" }}>
        <CardContent>
          <Typography variant="h5" gutterBottom>
            Sign in
          </Typography>
          <Stack spacing={2}>
            <TextField label="Email" type="email" fullWidth />
            <TextField label="Password" type="password" fullWidth />
            <Button variant="contained" fullWidth>
              Login (placeholder)
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Stack>
  );
}

export default LoginPage;

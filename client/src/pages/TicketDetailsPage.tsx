import { Alert, Card, CardContent, Divider, List, ListItem, ListItemText, Stack, Typography } from "@mui/material";
import { useState } from "react";

type ViewState = "loading" | "error" | "data";

function TicketDetailsPage(): JSX.Element {
  const [viewState] = useState<ViewState>("data");

  if (viewState === "loading")
  {
    return <Typography>Loading ticket...</Typography>;
  }

  if (viewState === "error")
  {
    return <Alert severity="error">Failed to load ticket details (stub).</Alert>;
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h4">Ticket #1</Typography>

      <Card>
        <CardContent>
          <Typography variant="h6">Cannot access LMS</Typography>
          <Typography color="text.secondary">Status: New</Typography>
          <Typography color="text.secondary">Priority: High</Typography>
          <Typography sx={{ mt: 2 }}>
            Student reports that LMS login returns "invalid credentials" despite password reset.
          </Typography>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Comments (stub)
          </Typography>
          <Divider sx={{ mb: 1 }} />
          <List>
            <ListItem disableGutters>
              <ListItemText primary="Student: Please help, assignment deadline today." secondary="2h ago" />
            </ListItem>
            <ListItem disableGutters>
              <ListItemText primary="Operator: Investigating authentication logs." secondary="1h ago" />
            </ListItem>
          </List>
        </CardContent>
      </Card>
    </Stack>
  );
}

export default TicketDetailsPage;

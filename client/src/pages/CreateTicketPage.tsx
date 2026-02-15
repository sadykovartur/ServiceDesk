import { Button, Card, CardContent, FormControl, InputLabel, MenuItem, Select, Stack, TextField, Typography } from "@mui/material";

function CreateTicketPage(): JSX.Element {
  return (
    <Stack spacing={2}>
      <Typography variant="h4">Create Ticket</Typography>
      <Card>
        <CardContent>
          <Stack spacing={2}>
            <TextField label="Title" fullWidth />
            <TextField label="Description" fullWidth multiline minRows={4} />
            <FormControl fullWidth>
              <InputLabel id="category-label">Category</InputLabel>
              <Select labelId="category-label" label="Category" defaultValue="">
                <MenuItem value="">Select category</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth>
              <InputLabel id="priority-label">Priority</InputLabel>
              <Select labelId="priority-label" label="Priority" defaultValue="Medium">
                <MenuItem value="Low">Low</MenuItem>
                <MenuItem value="Medium">Medium</MenuItem>
                <MenuItem value="High">High</MenuItem>
              </Select>
            </FormControl>
            <Button variant="contained">Submit (placeholder)</Button>
          </Stack>
        </CardContent>
      </Card>
    </Stack>
  );
}

export default CreateTicketPage;

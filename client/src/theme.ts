import { createTheme } from "@mui/material";

export const appTheme = createTheme({
  palette: {
    primary: {
      main: "#1976d2"
    }
  },
  typography: {
    fontFamily: "'Inter', 'Roboto', 'Arial', sans-serif",
    h4: {
      fontWeight: 700
    },
    h6: {
      fontWeight: 600
    }
  }
});

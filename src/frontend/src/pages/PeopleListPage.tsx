import { useEffect, useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import {
  Container,
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Button,
  Box,
  Stack,
} from "@mui/material";
import api from "../apis/peopleApi";
import type { Person } from "../types";
import { useAuth } from "../contexts/AuthContext";
import { notify } from "../utils/notify";

export default function PeopleListPage() {
  const [people, setPeople] = useState<Person[]>([]);
  const { role } = useAuth();

  const canEditAndDelete = role === "HRAdmin" || role === "Manager";

  useEffect(() => {
    fetchPeople();
  }, []);

  const fetchPeople = async () => {
    const res = await api.get("/people");
    setPeople(res.data);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this person?")) return;
    await api.delete(`/people?id=${id}`);
    notify("Successfully deleted record", "success");
    fetchPeople();
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" gutterBottom>
          People Records
        </Typography>
        {canEditAndDelete && (
          <Button
            variant="contained"
            component={RouterLink}
            to="/people/add"
            sx={{ mb: 2 }}
          >
            Add Person
          </Button>
        )}
        {people.length > 0 ? (
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>First Name</TableCell>
                <TableCell>Last Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Position</TableCell>
                {canEditAndDelete && (
                  <TableCell align="right">Actions</TableCell>
                )}
              </TableRow>
            </TableHead>
            <TableBody>
              {people.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>{p.firstName}</TableCell>
                  <TableCell>{p.lastName}</TableCell>
                  <TableCell>{p.email}</TableCell>
                  <TableCell>{p.position}</TableCell>
                  {canEditAndDelete && (
                    <TableCell align="right">
                      <Stack
                        direction="row"
                        spacing={1}
                        justifyContent="flex-end"
                      >
                        <Button
                          variant="outlined"
                          size="small"
                          component={RouterLink}
                          to={`/people/${p.id}`}
                        >
                          Edit
                        </Button>
                        <Button
                          variant="outlined"
                          color="error"
                          size="small"
                          onClick={() => handleDelete(p.id)}
                        >
                          Delete
                        </Button>
                      </Stack>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        ) : (
          <Typography variant="body1" sx={{ mt: 2 }}>
            No records found.
          </Typography>
        )}
      </Box>
    </Container>
  );
}

import { useEffect, useState } from 'react'
import { Link as RouterLink } from 'react-router-dom'
import {
  Container,
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Button,
  Box
} from '@mui/material'
import api from '../api/axios'
import type { Person } from '../types'
import { useAuth } from '../contexts/AuthContext'

export default function PeopleListPage() {
  const [people, setPeople] = useState<Person[]>([])
  const { role } = useAuth()

  useEffect(() => {
    api.get('/people').then((res) => setPeople(res.data))
  }, [])

  const canEdit = role === 'HR Admin' || role === 'Manager'

  return (
    <Container maxWidth="md">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" gutterBottom>
          People
        </Typography>
        {canEdit && (
          <Button variant="contained" component={RouterLink} to="/people/new">
            Add Person
          </Button>
        )}
        <Table sx={{ mt: 2 }}>
          <TableHead>
            <TableRow>
              <TableCell>First Name</TableCell>
              <TableCell>Last Name</TableCell>
              <TableCell>Email</TableCell>
              {canEdit && <TableCell />}
            </TableRow>
          </TableHead>
          <TableBody>
            {people.map((p) => (
              <TableRow key={p.id}>
                <TableCell>{p.firstName}</TableCell>
                <TableCell>{p.lastName}</TableCell>
                <TableCell>{p.email}</TableCell>
                {canEdit && (
                  <TableCell>
                    <Button component={RouterLink} to={`/people/${p.id}`} size="small">
                      Edit
                    </Button>
                  </TableCell>
                )}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Box>
    </Container>
  )
}
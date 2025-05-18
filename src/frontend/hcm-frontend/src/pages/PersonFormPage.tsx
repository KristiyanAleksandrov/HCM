import { useNavigate, useParams } from 'react-router-dom'
import { useEffect, useState } from 'react'
import {
  Container,
  Typography,
  TextField,
  Button,
  Box
} from '@mui/material'
import api from '../api/axios'

export default function PersonFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const editing = !!id
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: ''
  })

  useEffect(() => {
    if (editing) {
      api.get(`/people/${id}`).then((res) => setForm(res.data))
    }
  }, [id])

  const handleChange = (e: any) => {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  const handleSubmit = async (e: any) => {
    e.preventDefault()
    if (editing) {
      await api.put(`/people?id=${id}`, form)
    } else {
      await api.post('/people', form)
    }
    navigate('/')
  }

  return (
    <Container maxWidth="sm">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" gutterBottom>
          {editing ? 'Edit Person' : 'Add Person'}
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            label="First Name"
            name="firstName"
            fullWidth
            margin="normal"
            value={form.firstName}
            onChange={handleChange}
          />
          <TextField
            label="Last Name"
            name="lastName"
            fullWidth
            margin="normal"
            value={form.lastName}
            onChange={handleChange}
          />
          <TextField
            label="Email"
            name="email"
            type="email"
            fullWidth
            margin="normal"
            value={form.email}
            onChange={handleChange}
          />
          <Button variant="contained" type="submit" sx={{ mt: 2 }}>
            Save
          </Button>
        </form>
      </Box>
    </Container>
  )
}

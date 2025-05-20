import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Button,
  TextField,
  Container,
  Typography,
  Box,
  FormControl,
  FormGroup,
  FormControlLabel,
  Checkbox
} from '@mui/material'
import api from '../apis/authApi'

const availableRoles = ['Employee', 'Manager', 'HRAdmin']

export default function RegisterPage() {
  const navigate = useNavigate()
  const [form, setForm] = useState({
    username: '',
    email: '',
    password: '',
    roles: [] as string[]
  })
  const [error, setError] = useState('')

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  const handleRoleToggle = (role: string) => {
    setForm((prev) => {
      const roles = prev.roles.includes(role)
        ? prev.roles.filter((r) => r !== role)
        : [...prev.roles, role]
      return { ...prev, roles }
    })
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await api.post('/auth/register', form)
      navigate('/login')
    } catch {
      setError('Registration failed')
    }
  }

  return (
    <Container maxWidth="sm">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" gutterBottom>
          Register
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            label="Username"
            name="username"
            fullWidth
            margin="normal"
            value={form.username}
            onChange={handleChange}
            required
          />
          <TextField
            label="Email"
            name="email"
            fullWidth
            margin="normal"
            value={form.email}
            onChange={handleChange}
            required
          />
          <TextField
            label="Password"
            name="password"
            type="password"
            fullWidth
            margin="normal"
            value={form.password}
            onChange={handleChange}
            required
          />

          <FormControl component="fieldset" margin="normal">
            <Typography variant="subtitle1" gutterBottom>
              Roles
            </Typography>
            <FormGroup row>
              {availableRoles.map((role) => (
                <FormControlLabel
                  key={role}
                  control={
                    <Checkbox
                      checked={form.roles.includes(role)}
                      onChange={() => handleRoleToggle(role)}
                      name={role}
                    />
                  }
                  label={role}
                />
              ))}
            </FormGroup>
          </FormControl>

          {error && (
            <Typography color="error" variant="body2">
              {error}
            </Typography>
          )}

          <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
            Register
          </Button>
        </form>
      </Box>
    </Container>
  )
}

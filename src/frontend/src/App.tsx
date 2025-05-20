import { Routes, Route, Navigate, useNavigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import PeopleListPage from './pages/PeopleListPage'
import PersonFormPage from './pages/PersonFormPage'
import { useAuth } from './contexts/AuthContext'
import type { ReactNode } from 'react'
import { AppBar, Box, Button, Toolbar, Typography } from '@mui/material'

interface PrivateRouteProps {
  children: ReactNode
}

function PrivateRoute({ children}: PrivateRouteProps) {
  const { token } = useAuth()
  return token ? children : <Navigate to="/login" />
}

export default function App() {
  const { token, user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }
  
  return (
    <>
    {token && (
        <AppBar position="static" color="primary">
          <Toolbar sx={{ justifyContent: 'space-between' }}>
            <Typography onClick={() => navigate('/')} variant="h6" sx={{ cursor: 'pointer' }}>HCM App</Typography>
            <Box display="flex" alignItems="center" gap={4}>
              <Typography>{user}</Typography>
              <Button color="inherit" onClick={handleLogout}>
                Logout
              </Button>
            </Box>
          </Toolbar>
        </AppBar>
      )}
    <Box
      display="flex"
      justifyContent="center"
      alignItems="start"   
      minHeight="100vh"
      minWidth="100vw"
      bgcolor="#f4f6f8"
      pt={15} 
    >
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/"
            element={
              <PrivateRoute>
                <PeopleListPage />
              </PrivateRoute>
            }
          />
          <Route
            path="/people/add"
            element={
              <PrivateRoute>
                <PersonFormPage />
              </PrivateRoute>
            }
          />
          <Route
            path="/people/:id"
            element={
              <PrivateRoute>
                <PersonFormPage />
              </PrivateRoute>
            }
          />
        </Routes>
    </Box>
    </>
  )
}
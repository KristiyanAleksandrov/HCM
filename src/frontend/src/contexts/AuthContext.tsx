import { createContext, useContext, useState  } from 'react'
import type { ReactNode } from 'react'
import { jwtDecode } from 'jwt-decode'
import api from '../apis/authApi'

interface AuthContextValue {
  token: string | null
  login: (email: string, password: string) => Promise<void>
  logout: () => void
  role: string | null
}

const AuthContext = createContext<AuthContextValue>({
  token: null,
  login: async () => {},
  logout: () => {},
  role: null
})

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  )
  const [role, setRole] = useState<string | null>(
    token ? (jwtDecode(token) as any).role : null
  )

  const login = async (username: string, password: string) => {
    const { data } = await api.post('/auth/login', { username, password })
    localStorage.setItem('token', data.token)
    setToken(data.token)
    setRole((jwtDecode(data.token) as any).role)
  }

  const logout = () => {
    localStorage.removeItem('token')
    setToken(null)
    setRole(null)
  }

  return (
    <AuthContext.Provider value={{ token, login, logout, role }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
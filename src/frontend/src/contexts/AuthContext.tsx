import { createContext, useContext, useEffect, useState } from "react";
import type { ReactNode } from "react";
import { jwtDecode } from "jwt-decode";
import api from "../apis/authApi";

interface AuthContextValue {
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  role: string | null;
  user: string | null;
}

interface DecodedToken {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  exp: number;
  iss: string;
  aud: string;
}

const AuthContext = createContext<AuthContextValue>({
  token: null,
  login: async () => {},
  logout: () => {},
  user: null,
  role: null,
});

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );

  const [user, setUser] = useState<string | null>(
    token
      ? jwtDecode<DecodedToken>(token)[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ]
      : null
  );

  const [role, setRole] = useState<string | null>(
    token
      ? jwtDecode<DecodedToken>(token)[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ]
      : null
  );

  const login = async (username: string, password: string) => {
    const { data } = await api.post("/auth/login", { username, password });
    localStorage.setItem("token", data.token);
    setToken(data.token);

    const decoded = jwtDecode<DecodedToken>(data.token);
    setUser(
      decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
    );
    setRole(
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
    );
  };

  const logout = () => {
    localStorage.removeItem("token");
    setToken(null);
    setRole(null);
    setUser(null);
  };

  useEffect(() => {
  setGlobalLogout(logout)
  }, [logout])

  return (
    <AuthContext.Provider value={{ token, login, user, logout, role }}>
      {children}
    </AuthContext.Provider>
  );
}

let globalLogout: () => void = () => {}

export function setGlobalLogout(fn: () => void) {
  globalLogout = fn
}

export function getGlobalLogout() {
  console.log('[getGlobalLogout] Called')
  return globalLogout
}

export const useAuth = () => useContext(AuthContext);

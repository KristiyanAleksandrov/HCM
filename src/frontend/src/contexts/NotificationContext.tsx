import { createContext, useContext, useState, type ReactNode } from 'react'
import { Snackbar, Alert, type AlertColor, Slide, type SlideProps } from '@mui/material'
import { setNotifyImpl } from '../utils/notify'

interface NotificationContextValue {
  showMessage: (message: string, severity?: AlertColor) => void
}

const NotificationContext = createContext<NotificationContextValue>({
  showMessage: () => {}
})

function SlideTransition(props: SlideProps) {
  return <Slide {...props} direction="left" />
}

export function NotificationProvider({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState(false)
  const [message, setMessage] = useState('')
  const [severity, setSeverity] = useState<AlertColor>('info')

  const showMessage = (msg: string, type: AlertColor = 'info') => {
    setMessage(msg)
    setSeverity(type)
    setOpen(true)
  }
  setNotifyImpl(showMessage);

  const handleClose = () => setOpen(false)

  return (
    <NotificationContext.Provider value={{ showMessage }}>
      {children}
      <Snackbar
        open={open}
        autoHideDuration={6000}
        onClose={handleClose}
        anchorOrigin={{ vertical: "top", horizontal: "right" }}
        TransitionComponent={SlideTransition}
        sx={{ mt: '70px' }} 
      >
        <Alert severity={severity} onClose={handleClose} sx={{ width: "100%" }}>
          {message}
        </Alert>
      </Snackbar>
    </NotificationContext.Provider>
  );
}

export const useNotification = () => useContext(NotificationContext)
import { type AlertColor } from '@mui/material';

let notifyImpl: (msg: string, type?: AlertColor) => void = () => {};

export function notify(msg: string, type: AlertColor = 'info') {
  notifyImpl(msg, type);
}

export function setNotifyImpl(fn: typeof notifyImpl) {
  notifyImpl = fn;
}
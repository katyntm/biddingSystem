export const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount);
};

export const formatDateTime = (dateString: string): string => {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

export const getTimeRemaining = (endTimeString: string): string => {
  const now = new Date();
  const endTime = new Date(endTimeString);
  
  if (endTime <= now) {
    return 'Ended';
  }
  
  const totalSeconds = Math.floor((endTime.getTime() - now.getTime()) / 1000);
  const days = Math.floor(totalSeconds / 86400);
  const hours = Math.floor((totalSeconds % 86400) / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  
  if (days > 0) {
    return `${days}d ${hours}h left`;
  } else if (hours > 0) {
    return `${hours}h ${minutes}m left`;
  } else {
    return `${minutes}m left`;
  }
};

export const getStatusVariant = (status: string) => {
  switch (status) {
    case 'Won': return 'success';
    case 'Active': return 'primary';
    case 'Outbid': return 'warning';
    case 'Lost': return 'danger';
    default: return 'secondary';
  }
};


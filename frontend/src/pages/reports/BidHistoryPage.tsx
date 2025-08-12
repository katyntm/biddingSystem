import { Table, Card, Spinner, Alert, Badge } from 'react-bootstrap';
import { useBidReports } from '../../hooks/useReports';

const BidHistoryPage = () => {
  const { data: bidReports = [], isLoading, error, isError } = useBidReports();

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  };

  const getStatusVariant = (status: string) => {
    switch (status) {
      case 'Won': return 'success';
      case 'Active': return 'primary';
      case 'Outbid': return 'warning';
      case 'Lost': return 'danger';
      default: return 'secondary';
    }
  };

  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <Spinner animation="border" variant="primary" />
        <span className="ms-2">Loading bid reports...</span>
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="danger" className="m-4">
        <Alert.Heading>Error</Alert.Heading>
        <p>{error?.message || 'Failed to load bid reports'}</p>
      </Alert>
    );
  }

  return (
    <div className="bid-reports-page">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="mb-0">Bid History ({bidReports.length})</h2>
      </div>

      <Card className="shadow-sm">
        <Card.Header className="bg-light">
          <h5 className="mb-0">Your Bidding History</h5>
        </Card.Header>
        <Card.Body className="p-0">
          {bidReports.length === 0 ? (
            <div className="text-center p-5">
              <p className="text-muted mb-0">No bid reports found</p>
            </div>
          ) : (
            <div className="table-responsive">
              <Table striped hover className="mb-0">
                <thead className="table-dark">
                  <tr>
                    <th>Description</th>
                    <th>VIN</th>
                    <th>My Highest Bid</th>
                    <th>Price</th>
                    <th>Date</th>
                    <th>Sales Channel</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {bidReports.map((report) => (
                    <tr key={report.id}>
                      <td>
                        <strong>{report.year} {report.make} {report.model}</strong>
                      </td>
                      <td>
                        <code className="text-primary">{report.vin}</code>
                      </td>
                      <td>
                        <strong className="text-info">
                          {formatCurrency(report.bidAmount)}
                        </strong>
                      </td>
                      <td>
                        {report.currentHighestBid && (
                          <strong className="text-success">
                            {formatCurrency(report.currentHighestBid)}
                          </strong>
                        )}
                      </td>
                      <td>{formatDate(report.bidDate)}</td>
                      <td>
                        <Badge bg="secondary" className="px-2 py-1">
                          Online Auction
                        </Badge>
                      </td>
                      <td>
                        <Badge bg={getStatusVariant(report.status)} className="px-2 py-1">
                          {report.status}
                        </Badge>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

export default BidHistoryPage;
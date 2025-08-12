// import { useState, useEffect } from 'react';
import { Table, Card, Spinner, Alert } from "react-bootstrap";
import { usePurchaseReports } from "../../hooks/useReports";

const PurchaseHistoryPage = () => {
  const { data: purchaseReports = [], isLoading, error, isError } = usePurchaseReports();

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "400px" }}>
        <Spinner animation="border" variant="primary" />
        <span className="ms-2">Loading purchase history...</span>
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="danger" className="m-4">
        <Alert.Heading>Error</Alert.Heading>
        <p>{error?.message || "Failed to load purchase history"}</p>
      </Alert>
    );
  }

  return (
    <div className="purchase-reports-page">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="mb-0">Purchase History ({purchaseReports.length})</h2>
      </div>

      <Card className="shadow-sm">
        <Card.Body className="p-0">
          {purchaseReports.length === 0 ? (
            <div className="text-center p-5">
              <p className="text-muted mb-0">No purchase history found</p>
            </div>
          ) : (
            <div className="table-responsive">
              <Table striped hover className="mb-0">
                <thead className="table-dark">
                  <tr>
                    <th>Description</th>
                    <th>VIN</th>
                    <th>Sale Price</th>
                    <th>Purchase Date</th>
                  </tr>
                </thead>
                <tbody>
                  {purchaseReports.map((report) => (
                    <tr key={report.id}>
                      <td>
                        <strong>
                          {report.year} {report.make} {report.model}
                        </strong>
                      </td>
                      <td>
                        <code className="text-primary">{report.vin}</code>
                      </td>
                      <td>
                        <strong className="text-success">{formatCurrency(report.purchasePrice)}</strong>
                      </td>
                      <td>{formatDate(report.purchaseDate)}</td>
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

export default PurchaseHistoryPage;

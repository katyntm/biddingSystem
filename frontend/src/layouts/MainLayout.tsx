
import { Outlet } from 'react-router-dom';
import { Navbar, Nav, Container } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';

const MainLayout = () => {
  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    window.location.href = '/auth/login';
  };

  return (
    <div className="main-layout">
      <Navbar bg="warning" variant="light" expand="lg" className="shadow-sm">
        <Container>
          <Navbar.Brand href="/vehicles">
            <strong>Vehicle Auction</strong>
          </Navbar.Brand>
          
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="ms-auto">
              <LinkContainer to="/dashboard">
                <Nav.Link>Dashboard</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/vehicles">
                <Nav.Link>Vehicles</Nav.Link>
              </LinkContainer>
              {/* <LinkContainer to="/auctions">
                <Nav.Link>Auctions</Nav.Link>
              </LinkContainer> */}
              <LinkContainer to="/reports/bids">
                <Nav.Link>Bid History</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/reports/purchases">
                <Nav.Link>Purchase History</Nav.Link>
              </LinkContainer>
              <Nav.Link onClick={handleLogout} style={{ cursor: 'pointer' }}>
                Logout
              </Nav.Link>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      
      <main className="container-fluid p-4">
        <Outlet />
      </main>
    </div>
  );
};

export default MainLayout;
import { Navbar as BootstrapNavbar, Nav, Container } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';

const Navbar = () => {
  return (
    <BootstrapNavbar bg="warning" variant="light" expand="lg" className="shadow-sm">
      <Container>
        {/* <BootstrapNavbar.Brand href="/">
          <strong>Vehicle Auction</strong>
        </BootstrapNavbar.Brand> */}
        
        <BootstrapNavbar.Toggle aria-controls="basic-navbar-nav" />
        <BootstrapNavbar.Collapse id="basic-navbar-nav">
          <Nav className="ms-auto">
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
            <Nav.Link href="#" onClick={() => {
              localStorage.removeItem('accessToken');
              window.location.href = '/auth/login';
            }}>
              Logout
            </Nav.Link>
          </Nav>
        </BootstrapNavbar.Collapse>
      </Container>
    </BootstrapNavbar>
  );
};

export default Navbar;
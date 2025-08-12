import React, { useState, useEffect } from 'react';
import { Layout, Row, Col, Select, Button, Space, Typography, Spin, Alert, Pagination, Empty } from 'antd';
import { AppstoreOutlined, UnorderedListOutlined } from '@ant-design/icons';
import { useVehicles } from '../../api/vehicles.api';
import VehicleCard from '../../components/VehicleCard';
import VehicleFilters from '../../components/VehicleFilters';
import type { VehicleSearchParams } from '../../hooks/useVehicles';
import { toast } from 'react-toastify';

const { Content } = Layout;
const { Title, Text } = Typography;
const { Option } = Select;

const VehiclesPage: React.FC = () => {
  const [searchParams, setSearchParams] = useState<VehicleSearchParams>({
    page: 1,
    pageSize: 12,
    sortBy: 'year',
    sortOrder: 'desc'
  });
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');

  const { data: vehicleData, isLoading, error, isError } = useVehicles(searchParams);

  useEffect(() => {
    if (isError && error) {
      toast.error('Failed to load vehicles. Please try again.');
    }
  }, [isError, error]);

  const handleFiltersChange = (newFilters: VehicleSearchParams) => {
    setSearchParams(prev => ({
      ...prev,
      ...newFilters,
      page: 1, // Reset to first page when filters change
    }));
  };

  const handleClearFilters = () => {
    setSearchParams({
      page: 1,
      pageSize: 12,
      sortBy: 'year',
      sortOrder: 'desc'
    });
  };

  const handleSortChange = (value: string) => {
    const [sortBy, sortOrder] = value.split('-');
    setSearchParams(prev => ({
      ...prev,
      sortBy,
      sortOrder: sortOrder as 'asc' | 'desc',
      page: 1,
    }));
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setSearchParams(prev => ({
      ...prev,
      page,
      pageSize: pageSize || prev.pageSize,
    }));
  };

  const handleViewDetails = (vehicleId: string) => {
    // Navigate to vehicle details page
    window.open(`/vehicles/${vehicleId}`, '_blank');
  };

  const handleBid = (vehicleId: string) => {
    // Open bid modal or navigate to bid page
    toast.info(`Bidding functionality for vehicle ${vehicleId} will be implemented soon.`);
  };

  const sortOptions = [
    { value: 'year-desc', label: 'Year (Newest First)' },
    { value: 'year-asc', label: 'Year (Oldest First)' },
    { value: 'currentBid-asc', label: 'Price (Low to High)' },
    { value: 'currentBid-desc', label: 'Price (High to Low)' },
    { value: 'make-asc', label: 'Make (A-Z)' },
    { value: 'make-desc', label: 'Make (Z-A)' },
    { value: 'auctionEndDate-asc', label: 'Ending Soon' },
    { value: 'numberOfBids-desc', label: 'Most Bids' },
  ];

  if (isError) {
    return (
      <Layout style={{ minHeight: '100vh', padding: 24 }}>
        <Content>
          <Alert
            message="Error Loading Vehicles"
            description="Unable to load vehicles. Please check your connection and try again."
            type="error"
            showIcon
            action={
              <Button size="small" danger onClick={() => window.location.reload()}>
                Retry
              </Button>
            }
          />
        </Content>
      </Layout>
    );
  }

  return (
    <Layout style={{ minHeight: '100vh', padding: 24, backgroundColor: '#f5f5f5' }}>
      <Content>
        <div style={{ marginBottom: 24 }}>
          <Title level={2}>Vehicle Auction</Title>
          <Text type="secondary">Find and bid on your next vehicle</Text>
        </div>

        <VehicleFilters
          filters={searchParams}
          onFiltersChange={handleFiltersChange}
          onClearFilters={handleClearFilters}
          loading={isLoading}
        />

        <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <Text strong>
              {isLoading ? 'Loading...' : `${vehicleData?.totalCount || 0} vehicles found`}
            </Text>
            {vehicleData && vehicleData.totalCount > 0 && (
              <Text type="secondary" style={{ marginLeft: 8 }}>
                (Page {vehicleData.currentPage} of {vehicleData.totalPages})
              </Text>
            )}
          </div>

          <Space>
            <Text>Sort by:</Text>
            <Select
              value={`${searchParams.sortBy}-${searchParams.sortOrder}`}
              onChange={handleSortChange}
              style={{ width: 200 }}
              size="small"
            >
              {sortOptions.map(option => (
                <Option key={option.value} value={option.value}>
                  {option.label}
                </Option>
              ))}
            </Select>

            <Button.Group size="small">
              <Button
                type={viewMode === 'grid' ? 'primary' : 'default'}
                icon={<AppstoreOutlined />}
                onClick={() => setViewMode('grid')}
              />
              <Button
                type={viewMode === 'list' ? 'primary' : 'default'}
                icon={<UnorderedListOutlined />}
                onClick={() => setViewMode('list')}
              />
            </Button.Group>
          </Space>
        </div>

        <Spin spinning={isLoading}>
          {vehicleData?.items && vehicleData.items.length > 0 ? (
            <>
              <Row gutter={[16, 16]}>
                {vehicleData.items.map((vehicle) => (
                  <Col
                    key={vehicle.id}
                    xs={24}
                    sm={viewMode === 'grid' ? 12 : 24}
                    md={viewMode === 'grid' ? 8 : 24}
                    lg={viewMode === 'grid' ? 6 : 24}
                    xl={viewMode === 'grid' ? 6 : 24}
                  >
                    <VehicleCard
                      vehicle={vehicle}
                      onViewDetails={handleViewDetails}
                      onBid={handleBid}
                    />
                  </Col>
                ))}
              </Row>

              {vehicleData.totalPages > 1 && (
                <div style={{ marginTop: 32, textAlign: 'center' }}>
                  <Pagination
                    current={vehicleData.currentPage}
                    total={vehicleData.totalCount}
                    pageSize={vehicleData.pageSize}
                    onChange={handlePageChange}
                    showSizeChanger
                    showQuickJumper
                    showTotal={(total, range) =>
                      `${range[0]}-${range[1]} of ${total} vehicles`
                    }
                    pageSizeOptions={['12', '24', '48', '96']}
                  />
                </div>
              )}
            </>
          ) : !isLoading ? (
            <Empty
              description="No vehicles found"
              style={{ marginTop: 48 }}
            >
              <Button type="primary" onClick={handleClearFilters}>
                Clear Filters
              </Button>
            </Empty>
          ) : null}
        </Spin>
      </Content>
    </Layout>
  );
};

export default VehiclesPage;
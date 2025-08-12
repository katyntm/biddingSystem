import React from 'react';
import { Card,Button, Carousel, Typography, Space, Statistic, Tag } from 'antd';
import { ClockCircleOutlined,  EnvironmentOutlined, EyeOutlined } from '@ant-design/icons';
import type { Vehicle } from '../hooks/useVehicles';

const { Meta } = Card;
const { Text, Title } = Typography;

interface VehicleCardProps {
  vehicle: Vehicle;
  onViewDetails: (vehicleId: string) => void;
  onBid: (vehicleId: string) => void;
}

const VehicleCard: React.FC<VehicleCardProps> = ({ vehicle, onViewDetails, onBid }) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'green';
      case 'Sold': return 'red';
      case 'Pending': return 'orange';
      default: return 'default';
    }
  };

  const getConditionColor = (condition: string) => {
    switch (condition) {
      case 'Excellent': return 'green';
      case 'Good': return 'blue';
      case 'Fair': return 'orange';
      case 'Poor': return 'red';
      default: return 'default';
    }
  };

  return (
    <Card
      hoverable
      style={{ width: '100%', marginBottom: 16 }}
      cover={
        <div style={{ height: 200, overflow: 'hidden' }}>
          <Carousel autoplay>
            {vehicle.images.map((image, index) => (
              <div key={index}>
                <img
                  alt={`${vehicle.year} ${vehicle.make} ${vehicle.model}`}
                  src={image}
                  style={{
                    width: '100%',
                    height: 200,
                    objectFit: 'cover',
                  }}
                  onError={(e) => {
                    // Fallback to placeholder image
                    e.currentTarget.src = 'https://via.placeholder.com/400x200?text=No+Image';
                  }}
                />
              </div>
            ))}
          </Carousel>
        </div>
      }
      actions={[
        <Button 
          type="default" 
          icon={<EyeOutlined />} 
          onClick={() => onViewDetails(vehicle.id)}
          size="small"
        >
          View Details
        </Button>,
        <Button 
          type="primary" 
          disabled={vehicle.status !== 'Active'}
          onClick={() => onBid(vehicle.id)}
          size="small"
        >
          {vehicle.buyItNowPrice ? 'Buy Now' : 'Bid'}
        </Button>,
      ]}
    >
      <div style={{ marginBottom: 8 }}>
        <Space>
          <Tag color={getStatusColor(vehicle.status)}>{vehicle.status}</Tag>
          <Tag color={getConditionColor(vehicle.condition)}>{vehicle.condition}</Tag>
          <Tag>{vehicle.saleChannel}</Tag>
        </Space>
      </div>

      <Meta
        title={
          <Title level={4} style={{ margin: 0 }}>
            {vehicle.year} {vehicle.make} {vehicle.model}
          </Title>
        }
        description={
          <div>
            <Text type="secondary" style={{ fontSize: '12px' }}>
              VIN: {vehicle.vin}
            </Text>
            <br />
            <Text ellipsis style={{ fontSize: '13px' }}>
              {vehicle.description}
            </Text>
          </div>
        }
      />

      <div style={{ marginTop: 12 }}>
        <Space direction="vertical" size="small" style={{ width: '100%' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Text strong>Current Bid:</Text>
            <Statistic
              value={vehicle.currentBid}
              formatter={(value) => formatCurrency(Number(value))}
              valueStyle={{ fontSize: '16px', color: '#52c41a' }}
            />
          </div>

          {vehicle.buyItNowPrice && (
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Text strong>Buy It Now:</Text>
              <Statistic
                value={vehicle.buyItNowPrice}
                formatter={(value) => formatCurrency(Number(value))}
                valueStyle={{ fontSize: '14px', color: '#1890ff' }}
              />
            </div>
          )}

          <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '12px' }}>
            <Space>
              <ClockCircleOutlined />
              <Text>{vehicle.timeRemaining}</Text>
            </Space>
            <Text>{vehicle.numberOfBids} bids</Text>
          </div>

          <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '12px' }}>
            <Space>
              <EnvironmentOutlined />
              <Text>{vehicle.location}</Text>
            </Space>
            <Text>{vehicle.mileage.toLocaleString()} miles</Text>
          </div>
        </Space>
      </div>
    </Card>
  );
};

export default VehicleCard;
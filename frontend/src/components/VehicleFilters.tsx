import React from 'react';
import { Card, Form, Select, Input, InputNumber, Button, Space, Row, Col } from 'antd';
import { SearchOutlined, ClearOutlined } from '@ant-design/icons';
import type { VehicleSearchParams } from '../hooks/useVehicles';

const { Option } = Select;

interface VehicleFiltersProps {
  filters: VehicleSearchParams;
  onFiltersChange: (filters: VehicleSearchParams) => void;
  onClearFilters: () => void;
  loading?: boolean;
}

const VehicleFilters: React.FC<VehicleFiltersProps> = ({
  filters,
  onFiltersChange,
  onClearFilters,
  loading = false
}) => {
  const [form] = Form.useForm();

  const handleFormChange = (changedValues: any, allValues: any) => {
    // Clean up empty arrays and undefined values
    const cleanedValues = Object.entries(allValues).reduce((acc, [key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        if (Array.isArray(value) && value.length === 0) {
          // Don't include empty arrays
          return acc;
        }
        acc[key] = value;
      }
      return acc;
    }, {} as any);

    onFiltersChange(cleanedValues);
  };

  const handleClear = () => {
    form.resetFields();
    onClearFilters();
  };

  const makeOptions = ['Honda', 'Ford', 'Toyota', 'Chevrolet', 'BMW', 'Audi', 'Mercedes', 'Lexus'];
  const modelOptions = ['Civic', 'Escape', 'Camry', 'Tahoe', 'X3', 'A4', 'C-Class', 'RX'];
  const conditionOptions = ['Excellent', 'Good', 'Fair', 'Poor'];
  const statusOptions = ['Active', 'Sold', 'Pending'];

  return (
    <Card title="Filters" size="small" style={{ marginBottom: 16 }}>
      <Form
        form={form}
        layout="vertical"
        initialValues={filters}
        onValuesChange={handleFormChange}
        size="small"
      >
        <Row gutter={16}>
          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item name="search" label="Search">
              <Input
                placeholder="Search by make, model, VIN..."
                suffix={<SearchOutlined />}
                allowClear
              />
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item name="make" label="Make">
              <Select
                mode="multiple"
                placeholder="Select makes"
                allowClear
                maxTagCount="responsive"
              >
                {makeOptions.map(make => (
                  <Option key={make} value={make}>{make}</Option>
                ))}
              </Select>
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item name="model" label="Model">
              <Select
                mode="multiple"
                placeholder="Select models"
                allowClear
                maxTagCount="responsive"
              >
                {modelOptions.map(model => (
                  <Option key={model} value={model}>{model}</Option>
                ))}
              </Select>
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item name="condition" label="Condition">
              <Select
                mode="multiple"
                placeholder="Select conditions"
                allowClear
                maxTagCount="responsive"
              >
                {conditionOptions.map(condition => (
                  <Option key={condition} value={condition}>{condition}</Option>
                ))}
              </Select>
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item name="status" label="Status">
              <Select
                mode="multiple"
                placeholder="Select status"
                allowClear
                maxTagCount="responsive"
              >
                {statusOptions.map(status => (
                  <Option key={status} value={status}>{status}</Option>
                ))}
              </Select>
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={4} lg={3}>
            <Form.Item name="yearFrom" label="Year From">
              <InputNumber
                placeholder="2000"
                min={1900}
                max={new Date().getFullYear() + 1}
                style={{ width: '100%' }}
              />
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={4} lg={3}>
            <Form.Item name="yearTo" label="Year To">
              <InputNumber
                placeholder="2024"
                min={1900}
                max={new Date().getFullYear() + 1}
                style={{ width: '100%' }}
              />
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={4} lg={3}>
            <Form.Item name="priceFrom" label="Price From">
              <InputNumber
                placeholder="5000"
                min={0}
                formatter={(value) => `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                parser={(value) => value!.replace(/\$\s?|(,*)/g, '')}
                style={{ width: '100%' }}
              />
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={4} lg={3}>
            <Form.Item name="priceTo" label="Price To">
              <InputNumber
                placeholder="50000"
                min={0}
                formatter={(value) => `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                parser={(value) => value!.replace(/\$\s?|(,*)/g, '')}
                style={{ width: '100%' }}
              />
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={8} lg={6}>
            <Form.Item label=" ">
              <Space>
                <Button 
                  icon={<ClearOutlined />} 
                  onClick={handleClear}
                  disabled={loading}
                >
                  Clear All
                </Button>
              </Space>
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Card>
  );
};

export default VehicleFilters;
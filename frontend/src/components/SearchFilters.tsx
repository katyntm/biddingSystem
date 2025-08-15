import React from "react";
import { Form, Input, Select, Button, Row, Col, InputNumber } from "antd";
import type { VehicleSearchParams } from "../types/vehicle.types";

interface SearchFiltersProps {
  onSearch: (params: Partial<VehicleSearchParams>) => void;
}

const SearchFilters: React.FC<SearchFiltersProps> = ({ onSearch }) => {
  const [form] = Form.useForm();

  const handleSearch = () => {
    form.validateFields().then((values) => {
      onSearch(values);
    });
  };

  const handleReset = () => {
    form.resetFields();
    onSearch({});
  };

  return (
    <Form form={form} layout="vertical">
      <Row gutter={16}>
        <Col span={6}>
          <Form.Item name="make" label="Make">
            <Select placeholder="Select Make">
              <Select.Option value="Tesla">Tesla</Select.Option>
              <Select.Option value="BMW">BMW</Select.Option>
              <Select.Option value="Toyota">Toyota</Select.Option>
              <Select.Option value="Ford">Ford</Select.Option>
              <Select.Option value="Honda">Honda</Select.Option>
              <Select.Option value="Chevrolet">Chevrolet</Select.Option>
              <Select.Option value="Audi">Audi</Select.Option>
              <Select.Option value="Mercedes-Benz">Mercedes-Benz</Select.Option>
              <Select.Option value="Volvo">Volvo</Select.Option>
              <Select.Option value="Hyundai">Hyundai</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="modelType" label="Model">
            <Input placeholder="Enter Model" />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="yearFrom" label="Year From">
            <InputNumber placeholder="2015" min={2010} max={2025} style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="yearTo" label="Year To">
            <InputNumber placeholder="2025" min={2010} max={2025} style={{ width: '100%' }} />
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={6}>
          <Form.Item name="priceFrom" label="Price From">
            <InputNumber placeholder="20000" min={0} style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="priceTo" label="Price To">
            <InputNumber placeholder="80000" min={0} style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="fuelType" label="Fuel Type">
            <Select placeholder="Select Fuel Type">
              <Select.Option value="Electric">Electric</Select.Option>
              <Select.Option value="Gasoline">Gasoline</Select.Option>
              <Select.Option value="Hybrid">Hybrid</Select.Option>
              <Select.Option value="Diesel">Diesel</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="bodyStyle" label="Body Style">
            <Select placeholder="Select Body Style">
              <Select.Option value="Sedan">Sedan</Select.Option>
              <Select.Option value="SUV">SUV</Select.Option>
              <Select.Option value="Hatchback">Hatchback</Select.Option>
              <Select.Option value="Coupe">Coupe</Select.Option>
              <Select.Option value="Truck">Truck</Select.Option>
            </Select>
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={8}>
          <Form.Item name="transmission" label="Transmission">
            <Select placeholder="Select Transmission">
              <Select.Option value="Manual">Manual</Select.Option>
              <Select.Option value="Automatic">Automatic</Select.Option>
              <Select.Option value="AWD Automatic">AWD Automatic</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col span={8}>
          <Form.Item name="location" label="Location">
            <Input placeholder="Enter Location" />
          </Form.Item>
        </Col>
        <Col span={8}>
          <div style={{ paddingTop: 30 }}>
            <Button type="primary" onClick={handleSearch} style={{ marginRight: 8 }}>
              Search
            </Button>
            <Button onClick={handleReset}>
              Reset
            </Button>
          </div>
        </Col>
      </Row>
    </Form>
  );
};

export default SearchFilters;
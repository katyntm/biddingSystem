import { z } from 'zod';

export const loginSchema = z.object({
  username: z
    .string()
    .min(1, 'Username is required')
    .min(3, 'Username must be at least 3 characters'),
  password: z
    .string()
    .min(1, 'Password is required')
    .min(6, 'Password must be at least 6 characters'),
});

export type LoginFormData = z.infer<typeof loginSchema>;

export const bidSchema = z.object({
  vehicleId: z.string().min(1, 'Vehicle ID is required'),
  bidAmount: z
    .number()
    .min(1, 'Bid amount must be greater than 0')
    .refine((val) => val % 1 === 0 || val.toString().split('.')[1]?.length <= 2, {
      message: 'Bid amount must have at most 2 decimal places',
    }),
});

export type BidFormData = z.infer<typeof bidSchema>;

export const vehicleSearchSchema = z.object({
  make: z.string().optional(),
  model: z.string().optional(),
  yearFrom: z
    .number()
    .min(1900, 'Year must be at least 1900')
    .max(new Date().getFullYear() + 1, 'Year cannot be in the future')
    .optional(),
  yearTo: z
    .number()
    .min(1900, 'Year must be at least 1900')
    .max(new Date().getFullYear() + 1, 'Year cannot be in the future')
    .optional(),
  priceFrom: z.number().min(0, 'Price must be positive').optional(),
  priceTo: z.number().min(0, 'Price must be positive').optional(),
  vin: z.string().optional(),
});

export type VehicleSearchFormData = z.infer<typeof vehicleSearchSchema>;
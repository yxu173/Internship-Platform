# Payment Integration with Paymob

This document explains how the payment system is integrated with Paymob for the Internship Platform.

## Overview

The payment system allows students to purchase premium roadmaps. It uses Paymob's Unified Checkout API for handling payments.

## Architecture

The payment flow consists of the following components:

1. **Frontend**: 
   - PaymentService.ts - Client service for initiating payments
   - Enrollment UI - User interface for enrolling in premium roadmaps

2. **Backend**:
   - PaymentController - API endpoints for initiating payments and handling callbacks
   - InitiateRoadmapPaymentCommand - Command for initiating a payment
   - ProcessPaymobCallbackCommand - Command for processing payment callbacks
   - PaymobPaymentService - Service for communicating with Paymob's API
   - Enrollment - Domain entity for tracking enrollment and payment status

## Payment Flow

1. User clicks "Enroll" on a premium roadmap
2. System creates an Enrollment record with "Pending" status
3. Frontend calls `POST /api/payments/initiate-roadmap-purchase/{roadmapId}`
4. Backend generates a payment intention with Paymob and returns a checkout URL
5. User is redirected to Paymob's checkout page
6. User completes payment on Paymob's checkout page
7. Paymob sends a callback to `POST /api/payments/paymob-callback`
8. Backend verifies the callback and updates the enrollment status to "Completed"
9. User gains access to the premium roadmap

## Configuration

Add the following settings to your appsettings.json:

```json
"PaymobSettings": {
  "ApiKey": "your_api_key_here",
  "SecretKey": "your_secret_key_here",
  "PublicKey": "your_public_key_here",
  "HmacSecret": "your_hmac_secret_here",
  "BaseUrl": "https://accept.paymob.com/v1",
  "CardIntegrationId": 123456,
  "WalletIntegrationId": 789012
}
```

## Paymob Account Setup

1. Sign up for a Paymob account at https://paymob.com
2. Generate API keys in the dashboard:
   - Navigate to Settings > Account Info
   - Generate API Key, Secret Key, and Public Key
   - Copy these values to your appsettings.json
3. Create payment integration IDs:
   - Go to Developers > Payment Integrations
   - Create integrations for your desired payment methods (Card, Wallet, etc.)
   - Copy the integration IDs to your appsettings.json
4. Set up the callback URL:
   - Go to Developers > Endpoints
   - Add your callback URL (e.g., https://yourdomain.com/api/payments/paymob-callback)

## Webhook Setup

Paymob will send transaction callbacks to your webhook endpoint. To configure:

1. Set up a publicly accessible endpoint at `/api/payments/paymob-callback`
2. Configure HMAC verification using your Paymob HMAC secret
3. Process successful transactions to update enrollment status

## Testing

Use the `GET /api/payments/test-cards` endpoint to get test card numbers for development.

Common test cards:
- Successful payment: 4000000000000002 (Visa)
- Insufficient funds: 4000000000000036 (Visa)

## Handling Errors

The system handles various error scenarios:

- Invalid payment configuration
- Network errors when contacting Paymob
- Invalid callbacks or missing HMAC
- Missing enrollment records
- Duplicate payments

## Security Considerations

- All API keys and secrets are stored in appsettings.json and not checked into source control
- HMAC verification is used to validate callbacks from Paymob
- Enrollment records are created before initiating payment to prevent fraud
- Payment status updates are idempotent to handle duplicate callbacks

## Database Schema

The Enrollment entity includes the following payment-related fields:

- PaymentStatus (Pending, Completed, Failed, Refunded)
- PaymentProviderTransactionId (the Paymob transaction ID)
- AmountPaid (the actual amount paid) 
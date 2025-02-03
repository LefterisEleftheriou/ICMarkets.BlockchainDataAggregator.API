# ICMarkets Blockchain Data Aggregator

## 📌 Overview
This project is a **.NET Core Web API** that aggregates blockchain data from the **BlockCypher API** for **Bitcoin, Ethereum, Litecoin, Dash**, and **Bitcoin Testnet**.

It follows **Clean Architecture** with separate layers:
- **API Layer** - Exposes RESTful endpoints.
- **Application Layer** - Implements business logic.
- **Domain Layer** - Defines core entities.
- **Infrastructure Layer** - Handles database access and external API calls.

## 🛠️ Technologies Used
- **.NET 8**
- **Entity Framework Core**
- **SQLite**
- **Moq & NUnit (Testing)**
- **Swagger (API Documentation)**
- **Performance Optimizations**

## 🚀 Getting Started

### 📥 Prerequisites
- .NET SDK 8

#### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/yourusername/ICMarkets-Blockchain-Aggregator.git
cd ICMarkets-Blockchain-Aggregator

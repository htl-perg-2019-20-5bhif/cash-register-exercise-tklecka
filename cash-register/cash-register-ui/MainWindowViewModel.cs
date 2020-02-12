﻿using cash_register.Data;
using Polly;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cash_register_ui
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly AsyncPolicy RetryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(5);

        public MainWindowViewModel()
        {
            // Connect the command with the handling function
            AddToBasketCommand = new DelegateCommand<int?>(OnAddToBasket);

            // Connect the command with the handling function AND define a function
            // that returns true only if the command can be executed (i.e. the button
            // can be pressed).
            CheckoutCommand = new DelegateCommand(async () => await OnCheckout(), () => Basket.Count > 0);

            // Whenever something in the shopping basket changes, we have to notify WPF
            // that the total sum has changed and the execution state of our checkout command
            // might have changed.
            Basket.CollectionChanged += (_, __) =>
            {
                CheckoutCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(TotalSum));
            };
        }

        private readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000"),
            Timeout = TimeSpan.FromSeconds(500)
        };

        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { SetProperty(ref products, value); }
        }

        private ObservableCollection<ReceiptLineViewModel> basket = new ObservableCollection<ReceiptLineViewModel>();
        public ObservableCollection<ReceiptLineViewModel> Basket
        {
            get { return basket; }
            set { SetProperty(ref basket, value); }
        }

        public decimal TotalSum => Basket.Sum(rl => rl.TotalPrice);

        // Note the use of `DelegateCommand` here. It is provided by Prism.
        // A delegate command is representing a function that should be called 
        // when the button is pressed *and* an *execution state*. If the *execution state* 
        // is `false`, the command cannot be called and WPF will *automatically disable 
        // the bound button*. If it is `true`, the button is enabled.
        // The command takes an integer input parameter because it receives the
        // ID of the product which should be added.
        public DelegateCommand<int?> AddToBasketCommand { get; }

        private void OnAddToBasket(int? productID)
        {
            // Lookup the product based on the ID
            var product = Products.First(p => p.ID == productID);

            // Check whether the product is already in the basket
            var basketItem = Basket.FirstOrDefault(p => p.ProductID == productID);
            if (basketItem != null)
            {
                // Product already in the basket -> add amount and total price
                basketItem.Amount++;
                basketItem.TotalPrice += product.Price;
            }
            else
            {
                // New product -> add item to basket
                Basket.Add(new ReceiptLineViewModel
                {
                    ProductID = product.ID,
                    Amount = 1,
                    ProductName = product.Name,
                    TotalPrice = product.Price
                });
            }
        }

        public DelegateCommand CheckoutCommand { get; }

        private async Task OnCheckout()
        {
            // Turn all items in the basket into DTO objects
            var dto = Basket.Select(b => new ReceiptLineDto
            {
                ProductID = b.ProductID,
                Amount = b.Amount
            }).ToList();

            // Create JSON content that can be sent using HTTP POST
            using (var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"))
            {
                // Send the receipt to the backend
                var response = await RetryPolicy.ExecuteAndCaptureAsync(async () => await HttpClient.PostAsync("/api/receipts", content));

                // Throw exception if something went wrong
                response.Result.EnsureSuccessStatusCode();
            }

            // Clear basket so shopping can start from scratch
            Basket.Clear();
        }

        public async Task InitAsync()
        {
            // Here, we use the NuGet package Polly to get failure handling and
            // retry. This is optional. If you want to implement it without retry,
            // that's fine, too. However, in practice you want retry policies in
            // case of shaky networks.
            var productsString = await RetryPolicy.ExecuteAndCaptureAsync(
                async () => await HttpClient.GetStringAsync("/api/products"));
            Products = JsonSerializer.Deserialize<ObservableCollection<Product>>(productsString.Result);
        }
    }
}

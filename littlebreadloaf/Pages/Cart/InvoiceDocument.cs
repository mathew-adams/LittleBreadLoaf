using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using littlebreadloaf.Data;
using System.IO;
namespace littlebreadloaf.Pages.Cart
{
    public class InvoiceDocument : IDocument
    {
        private string _logoPath;
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model, string logoPath)
        {
            Model = model;
            _logoPath = logoPath;
        }

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(30);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(masterRow =>
            {
                masterRow.RelativeItem().Column(master =>
                {
                    master.Item().Row(row =>
                    {
                        row.RelativeItem().Column(Column =>
                        {
                            Column.Item().Text("Invoice").FontFamily("Arial").Bold().FontSize(48);
                        });
                        var bytLogo = System.IO.File.ReadAllBytes(_logoPath);

                        row.ConstantItem(80).Height(80).Image(bytLogo);

                        row.RelativeItem().PaddingLeft(20).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.ConstantItem(40).Text("From");
                                r.RelativeItem().Column(c2 =>
                                {
                                    c2.Item().Text(Model.Name).Bold();
                                    c2.Item().Text(Model.AddressLine1);
                                    c2.Item().Text(Model.AddressLine2);
                                    c2.Item().Text(Model.Phone);
                                });
                            });
                        });
                    });
                    master.Item().PaddingTop(15).Row(row =>
                    {
                        var delivery = "";
                        var date = "";
                        if(Model.ProductOrder.DeliveryDate < new DateTime(9999, 12, 31))
                        {
                            delivery = "Delivery:";
                            date = Model.ProductOrder.DeliveryDate.Value.ToString("yyyy-MM-dd");
                        }else if(Model.ProductOrder.PickupDate < new DateTime(9999, 12, 31))
                        {
                            delivery = "Pickup:";
                            date = $"{Model.ProductOrder.PickupDate.Value.ToString("yyyy-MM-dd")} @ {Model.ProductOrder.PickupTime}" ;
                        }


                        row.RelativeItem().Row(detail =>
                        {
                            detail.ConstantItem(80).Column(c =>
                            {
                                c.Item().Text("Invoice ID:");
                                c.Item().Text("Issued Date:");
                                c.Item().Text("Due Date:");
                                c.Item().Text(delivery);
                                c.Item().Text("Status:");
                            });
                            detail.RelativeItem().Column(c => 
                            {
                                c.Item().Text(Model.ProductOrder.ConfirmationCode).Bold();
                                c.Item().Text(Model.Invoice.Created.ToString("yyyy-MM-dd"));
                                c.Item().Text(Model.Invoice.Due.ToString("yyyy-MM-dd"));
                                c.Item().Text(date);
                                if (Model.Status == "DUE")
                                {
                                    c.Item().Text(Model.Status).Bold().FontColor("bc051b");
                                }
                                else
                                {
                                    c.Item().Text(Model.Status).Bold().FontColor("037501");
                                }
                            });
                        });
                        row.ConstantItem(100).Height(100);
                        row.RelativeItem().PaddingLeft(20).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.ConstantItem(40).Text("To");
                                r.RelativeItem().Column(c2 =>
                                {
                                    c2.Item().Text(Model.ProductOrder.ContactName).Bold();
                                    c2.Item().Text($"Phone: {Model.ProductOrder.ContactPhone}");
                                    if(Model.HasAddress)
                                    {
                                        c2.Item().Text(Model.NzAddressDeliverable.address_number);
                                        c2.Item().Text(Model.NzAddressDeliverable.suburb_locality);
                                        c2.Item().Text(Model.NzAddressDeliverable.town_city);
                                    }
                                });
                            });
                        });
                    });
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {

                column.Item().Element(ComposeTable);

                var totalPrice = Model.InvoiceTransactions.Sum(x => x.Price * x.Quantity);
                column.Item().PaddingRight(5).AlignRight().Text($"Grand total: {totalPrice}", TextStyle.Default.SemiBold());

                column.Item().PaddingTop(25).Element(ComposeComments);

            });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(35);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("Type").Style(headerStyle);
                    header.Cell().Text("Name").Style(headerStyle);
                    header.Cell().Text("Description").Style(headerStyle);
                    header.Cell().AlignRight().Text("Unit price").Style(headerStyle);
                    header.Cell().AlignRight().Text("Quantity").Style(headerStyle);
                    header.Cell().AlignRight().Text("Amount").Style(headerStyle);

                    header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                foreach (var transaction in Model.InvoiceTransactions)
                {
                    table.Cell().Element(CellStyle).Text(transaction.Category);
                    table.Cell().Element(CellStyle).Text(transaction.Name);
                    table.Cell().Element(CellStyle).Text(transaction.Description);
                    table.Cell().Element(CellStyle).AlignRight().Text(transaction.Price);
                    table.Cell().Element(CellStyle).AlignRight().Text(transaction.Quantity);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.Price * transaction.Quantity}");
                }
            });
        }

        static IContainer CellStyle(IContainer container)
        {
            var cellStyle = TextStyle.Default.FontSize(9f);
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).DefaultTextStyle(cellStyle);
        }

        void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(5).Column(column =>
            {
                var style = TextStyle.Default.FontSize(10f);
                column.Spacing(5);
                column.Item().DefaultTextStyle(style).Text("Thank you for placing an order!").FontSize(12).SemiBold();
                column.Item().DefaultTextStyle(style).Text($@"
Our banking details are as follows:
    {Model.Name}
    {Model.BankNumber}
Please use your confirmation number as the reference
Thank you,
Lynda and Lillian
");
            });
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}

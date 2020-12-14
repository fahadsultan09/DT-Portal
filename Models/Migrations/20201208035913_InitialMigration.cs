using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Models.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ActionName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationModule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ModuleName = table.Column<string>(maxLength: 50, nullable: false),
                    ModuleIcon = table.Column<string>(maxLength: 50, nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationModule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    MemberName = table.Column<string>(maxLength: 255, nullable: true),
                    MethodBase = table.Column<string>(maxLength: 255, nullable: true),
                    MemberType = table.Column<string>(maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(maxLength: 255, nullable: true),
                    ClassName = table.Column<string>(maxLength: 255, nullable: true),
                    ExceptionMessage = table.Column<string>(maxLength: 255, nullable: true),
                    Source = table.Column<string>(maxLength: 255, nullable: true),
                    Trace = table.Column<string>(maxLength: 10000, nullable: true),
                    ErrorLineNumber = table.Column<string>(maxLength: 255, nullable: true),
                    ThrowLineNumber = table.Column<string>(maxLength: 255, nullable: true),
                    ColumnNumber = table.Column<string>(maxLength: 255, nullable: true),
                    HelpLink = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    SAPProductCode = table.Column<int>(nullable: false),
                    PackCode = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductDescription = table.Column<string>(nullable: true),
                    ProductPrice = table.Column<int>(nullable: false),
                    ProductOrigin = table.Column<string>(nullable: true),
                    PackSize = table.Column<string>(nullable: true),
                    Strength = table.Column<string>(nullable: true),
                    CartonSize = table.Column<int>(nullable: false),
                    SFSize = table.Column<int>(nullable: false),
                    TradePrice = table.Column<int>(nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    Discount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    RegionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    RoleName = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    ApplicationModuleId = table.Column<int>(nullable: false),
                    ControllerName = table.Column<string>(maxLength: 100, nullable: false),
                    PageTitle = table.Column<string>(maxLength: 100, nullable: false),
                    PageURL = table.Column<string>(maxLength: 200, nullable: false),
                    PageIcon = table.Column<string>(maxLength: 50, nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPage_ApplicationModule_ApplicationModuleId",
                        column: x => x.ApplicationModuleId,
                        principalTable: "ApplicationModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    ProductMasterId = table.Column<int>(nullable: false),
                    Visibility = table.Column<int>(nullable: false),
                    PlantLocation = table.Column<string>(nullable: true),
                    Company = table.Column<int>(nullable: false),
                    WTaxRate = table.Column<string>(nullable: true),
                    Factor = table.Column<int>(nullable: false),
                    ParentDistributor = table.Column<string>(nullable: true),
                    S_OrderType = table.Column<string>(nullable: true),
                    R_OrderType = table.Column<string>(nullable: true),
                    SaleOrganization = table.Column<string>(nullable: true),
                    Distribution = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    DispatchPlant = table.Column<string>(nullable: true),
                    S_StorageLocation = table.Column<string>(nullable: true),
                    R_StorageLocation = table.Column<string>(nullable: true),
                    SalesItemCategory = table.Column<string>(nullable: true),
                    ReturnItemCategory = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetail_ProductMaster_ProductMasterId",
                        column: x => x.ProductMasterId,
                        principalTable: "ProductMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubRegion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    SubRegionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubRegion_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    UserName = table.Column<string>(maxLength: 6, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    IsDistributor = table.Column<bool>(nullable: false),
                    DistributorId = table.Column<int>(nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPageAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ApplicationPageId = table.Column<int>(nullable: false),
                    ApplicationActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPageAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPageAction_ApplicationAction_ApplicationActionId",
                        column: x => x.ApplicationActionId,
                        principalTable: "ApplicationAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationPageAction_ApplicationPage_ApplicationPageId",
                        column: x => x.ApplicationPageId,
                        principalTable: "ApplicationPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SubRegionId = table.Column<int>(nullable: false),
                    CityName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_SubRegion_SubRegionId",
                        column: x => x.SubRegionId,
                        principalTable: "SubRegion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    ApplicationPageId = table.Column<int>(nullable: false),
                    ApplicationPageActionId = table.Column<int>(nullable: false),
                    ApplicationActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_ApplicationAction_ApplicationActionId",
                        column: x => x.ApplicationActionId,
                        principalTable: "ApplicationAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermission_ApplicationPageAction_ApplicationPageActionId",
                        column: x => x.ApplicationPageActionId,
                        principalTable: "ApplicationPageAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermission_ApplicationPage_ApplicationPageId",
                        column: x => x.ApplicationPageId,
                        principalTable: "ApplicationPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Distributor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DistributorSAPCode = table.Column<int>(nullable: false),
                    DistributorCode = table.Column<string>(nullable: true),
                    DistributorName = table.Column<string>(nullable: true),
                    DistributorAddress = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: false),
                    SubRegionId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    NTN = table.Column<string>(nullable: true),
                    CNIC = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributor_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Distributor_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Distributor_SubRegion_SubRegionId",
                        column: x => x.SubRegionId,
                        principalTable: "SubRegion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DistributorId = table.Column<int>(nullable: false),
                    ReferenceNo = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Attachment = table.Column<string>(nullable: true),
                    TotalValue = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMaster_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DistributorId = table.Column<int>(nullable: false),
                    DepositDate = table.Column<DateTime>(nullable: false),
                    ValueClearingDate = table.Column<DateTime>(nullable: false),
                    PaymentMode = table.Column<int>(nullable: false),
                    DepositorBankName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMaster_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_OrderMaster_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetail_ProductMaster_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPage_ApplicationModuleId",
                table: "ApplicationPage",
                column: "ApplicationModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPageAction_ApplicationActionId",
                table: "ApplicationPageAction",
                column: "ApplicationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPageAction_ApplicationPageId",
                table: "ApplicationPageAction",
                column: "ApplicationPageId");

            migrationBuilder.CreateIndex(
                name: "IX_City_SubRegionId",
                table: "City",
                column: "SubRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_CityId",
                table: "Distributor",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_RegionId",
                table: "Distributor",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_SubRegionId",
                table: "Distributor",
                column: "SubRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaster_DistributorId",
                table: "OrderMaster",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMaster_DistributorId",
                table: "PaymentMaster",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetail_ProductMasterId",
                table: "ProductDetail",
                column: "ProductMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_ApplicationActionId",
                table: "RolePermission",
                column: "ApplicationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_ApplicationPageActionId",
                table: "RolePermission",
                column: "ApplicationPageActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_ApplicationPageId",
                table: "RolePermission",
                column: "ApplicationPageId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubRegion_RegionId",
                table: "SubRegion",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptionLog");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "PaymentMaster");

            migrationBuilder.DropTable(
                name: "ProductDetail");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "OrderMaster");

            migrationBuilder.DropTable(
                name: "ProductMaster");

            migrationBuilder.DropTable(
                name: "ApplicationPageAction");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Distributor");

            migrationBuilder.DropTable(
                name: "ApplicationAction");

            migrationBuilder.DropTable(
                name: "ApplicationPage");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "ApplicationModule");

            migrationBuilder.DropTable(
                name: "SubRegion");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}

using GVMP;

namespace Crimelife
{
    public static class ComputerWindow
    {
        public static void OpenComputer(this DbPlayer dbPlayer)
        {
            // dbPlayer.PlayAnimation(49, "cellphone@", "cellphone_email_read_base");
            dbPlayer.TriggerEvent("openComputer");
        }

        public static void CloseComputer(this DbPlayer dbPlayer)
        {
            if (!dbPlayer.Client.IsInVehicle)
            {
                dbPlayer.StopAnimation();
            }
            dbPlayer.TriggerEvent("closeComputer");
        }

        public static void responseComputerApps(this DbPlayer dbPlayer, string args)
        {
            dbPlayer.TriggerEvent("componentServerEvent", "DesktopApp", "responseComputerApps", args);
        }

        public static void responseTenants(this DbPlayer dbPlayer, string args)
        {
            dbPlayer.TriggerEvent("componentServerEvent", "HouseList", "responseTenants", args);
        }
    }
}

/*                            case "PoliceAktenSearchApp":
                                this.pageStack.push(LD);
                                break;
                            case "MarketplaceApp":
                                this.pageStack.push(Co);
                                break;
                            case "FahrzeugUebersichtApp":
                                this.pageStack.push(wo);
                                break;
                            case "KennzeichenUebersichtApp":
                                this.pageStack.push(mo);
                                break;
                            case "VehicleClawUebersichtApp":
                                this.pageStack.push(ho);
                                break;
                            case "VehicleTaxApp":
                                this.pageStack.push(Zo);
                                break;
                            case "ServiceOverviewApp":
                                this.pageStack.push($o);
                                break;
                            case "VehicleImpoundApp":
                                this.pageStack.push(Mt);
                                break;
                            case "KFZRentApp":
                                this.pageStack.push(ot);
                                break;
                            case "HouseApp":
                                this.pageStack.push(Tt);
                                break;
                            case "FraktionListApp":
                                this.pageStack.push(Aa);
                                break;
                            case "EmailApp":
                                this.pageStack.push(Ma);
                                break;
                            case "ExportApp":
                                this.pageStack.push(aa);
                                break;
                            case "BusinessDetailApp":
                                this.pageStack.push(sa);
                                break;
                            case "StreifenApp":
                                this.pageStack.push(Ua);
                                break;*/
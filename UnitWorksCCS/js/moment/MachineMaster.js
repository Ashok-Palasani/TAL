$(document).ready(function () {
    GetPlant();

    function GetPlant() {
        $.get("/Machines/GetPlant", {}, function (msg) {
            var ddlPlant = $("#EditPlantID");
            ddlPlant.empty().append('<option selected="selected" value="0">--Select Plant--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    ddlPlant.append('<option  value="' + data[i].PlantID + '">' + data[i].PlantName + '</option>');
                }

            }

        });
    }
    function GetPlantDetails(PlantID,ShopId,CellID) {
        $.get("/Machines/GetPlant", {}, function (msg) {
            var ddlPlant = $("#EditPlantID");
            ddlPlant.empty().append('<option selected="selected" value="0">--Select Plant--</option>');
            if (msg != '') {
                data = JSON.parse(msg);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].PlantID == PlantID) {
                        ddlPlant.append('<option selected="selected"  value="' + data[i].PlantID + '">' + data[i].PlantName + '</option>');
                    }
                    else {
                        ddlPlant.append('<option  value="' + data[i].PlantID + '">' + data[i].PlantName + '</option>');
                    }                    
                }


            }
            GetshopDetails(ShopId,CellID);


        });

    }

    function GetshopDetails(ShopID,CellID) {
        PlantID = $("#EditPlantID").val();
        $.get("/Machines/GetShop", { PlantID }, function (msg) {
            var ddlshop = $("#EditShopID");
            ddlshop.empty().append('<option selected="selected" value="0">--Select Shop--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    if (data[i].ShopID == ShopID) {
                        ddlshop.append('<option selected="selected"  value="' + data[i].ShopID + '">' + data[i].ShopName + '</option>');
                    }
                    else {
                        ddlshop.append('<option  value="' + data[i].ShopID + '">' + data[i].ShopName + '</option>');
                    }
                }
            }
            GetCellDetails(CellID);

        });
    }

    function GetCellDetails(CellID) {
        ShopID = $("#EditShopID").val();
        $.get("/Machines/GetCell", { ShopID }, function (msg) {
            var ddlCell = $("#EditCellID");
            ddlCell.empty().append('<option selected="selected" value="0">--Select Cell--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    if (data[i].CellID == CellID) {
                        ddlCell.append('<option selected="selected"  value="' + data[i].CellID + '">' + data[i].CellName + '</option>');
                    }
                    else {
                        ddlCell.append('<option  value="' + data[i].CellID + '">' + data[i].CellName + '</option>');
                    }
                    
                }
            }

        });
    }

    function GetProgramDetails(PtypeID) {
        PtypeID = parseInt(PtypeID);
        $.get("/Machines/GetProgramTypeDetails", { PtypeID }, function (msg) {
            var ddlCell = $("#EditProgramType");
            ddlCell.empty().append('<option selected="selected" value="0">--Select Program Type--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    if (data[i].ptypeid == PtypeID) {
                        ddlCell.append('<option selected="selected"  value="' + data[i].ptypeid + '">' + data[i].TypeName + '</option>');
                    }
                    else {
                        ddlCell.append('<option  value="' + data[i].ptypeid + '">' + data[i].TypeName + '</option>');
                    }

                }
            }

        });
    }


    $(document).on("change","#EditPlantID",function (e) {

        var PlantID = $("#EditPlantID").val();
        $.get("/Machines/GetShop", { PlantID }, function (msg) {
            var ddlshop = $("#EditShopID");
            ddlshop.empty().append('<option selected="selected" value="0">--Select Shop--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    ddlshop.append('<option  value="' + data[i].ShopID + '">' + data[i].ShopName + '</option>');
                }

            }

        });
    });

     $(document).on("change","#EditShopID",function (e) {
        var ShopID = $("#EditShopID").val();

        $.get("/Machines/GetCell", { ShopID }, function (msg) {
            var ddlCell = $("#EditCellID");
            ddlCell.empty().append('<option selected="selected" value="0">--Select Cell--</option>');
            if (msg != '') {
                data = JSON.parse(msg);

                for (var i = 0; i < data.length; i++) {
                    ddlCell.append('<option  value="' + data[i].CellID + '">' + data[i].CellName + '</option>');
                }

            }

        });
    });

    //On change of ProgramType
    $(document).on("change", "#ProgramType", function (e) {
        //$("#ProgramType").on("change", function () {
        var id = this;
        //var prog = this.val();
        var progtype = $("#ProgramType").val();
        //var cssdata = "";
        $("#AddTextid").empty();
        if (progtype == "1") {

            var cssdata = '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">Ip Address</label><input type="Text" class="ipaddress" id="ipaddress"/>';

        }

        else if (progtype == "2") {
            $("#AddTextid").empty();
            //var cssdata = "";
            var cssdata = '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">Ip Address</label><input type="Text" class="ipaddress" id="ipaddress"/>' +
                '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">UserName</label> <input type="Text" class="Username" id="Username" />' +
                '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">PassWord</label> <input type="password" class="password" id="password" />';

        }

        else if (progtype == "3") {
            $("#AddTextid").empty();
            //var cssdata = "";
            var cssdata = '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">Ip Address</label><input type="Text" class="ipaddress" id="ipaddress"/>' +
                 '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">UserName</label> <input type="Text" class="Username" id="Username" />' +
                 '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">PassWord</label> <input type="password" class="password" id="password" />' +
                 '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">Port</label> <input type="number" class="port" id="port" />' +
                 '<label for="input-1" class="stacked-label" style="resize: horizontal;padding-top: 2%;padding-left: 14px;width: 10%;color: blue;font-style: oblique;">Domain</label> <input type="domain" class="domain" id="domain" />';

        }
        //$("#Textid").append(cssdata);

        $(cssdata).appendTo($('#AddTextid'));

    });

    //Add Machine details
    $(document).on("click", "#savebtn", function (e) {
        var ipaddress = "";
        var username = "";
        var password = "";
        var port = 0;
        var domain = "";
        var plantid = parseInt($("#plantID").val());
        var shopid = parseInt($("#DepartmentID").val());
        var cellid = parseInt($("#MachineCategoryID").val());
        var MachineInvNo = $("#MachineInvNo").val();
        var MachineModel = $("#MachineModel").val();
        var ControllerType = $("#ControllerType").val();
        var MachineDispName = $("#MachineDispName").val();
        var MachineMake = $("#MachineMake").val();

        var programtype = $("#ProgramType").val();
        if (programtype == 1) {
            ipaddress = $("#ipaddress").val();
        }
        else if (programtype == 2) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
        }
        else if (programtype == 3) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
            port = parseInt($("#port").val());
            domain = $("#domain").val();
        }
        $.post("/Machines/CreateData", { plantid, shopid, cellid, MachineInvNo, MachineModel, ControllerType, MachineDispName, MachineMake, programtype, ipaddress, username, password, port, domain }, function (msg) {
            if (msg == "Success") {
                alert("Machine details Created Successfully");
                window.location.href = "/Machines/MachineList";
            }

        });
    });

    //Update Machine Details
    $(document).on("click", "#btnupdate", function (e) {
        var ipaddress = "";
        var username = "";
        var password = "";
        var port = "";
        var domain = "";
        var id = $("#hdntxt").val();
        var plantid = $("#EditPlant").val();
        var shopid = $("#Editdeptid").val();
        var cellid = $("#EditMachineCategory").val();
        var MachineInvNo = $("#EditMachineInvNo").val();
        var ControllerType = $("#EditControllerType").val();
        var MachineDispName = $("#EditMachineDispName").val();
        var MachineMake = $("#EditMachineMake").val();
        var MachineModel = $("#EditMachineModel").val();

        var programtype = $("#EditProgramType").val();
        if (programtype == 1) {
            ipaddress = $("#ipaddress").val();
        }
        else if (programtype == 2) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
        }
        else if (programtype == 3) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
            port = $("#port").val();
            domain = $("#domain").val();
        }
        $.get("/Machines/EditData", { id, plantid, shopid, cellid, MachineInvNo, ControllerType, MachineDispName, MachineMake: MachineMake, MachineModel, programtype: programtype, ipaddress, username, password, port, domain }, function (data) {
            if (data == "Success") {
                alert("item Updated Successfully");
                window.location.href = "/Machines/MachineList";
            }

        });
    });

    //Edit Machinedetails
    $(document).on("click", '.MachineEdit', function (e) {
        var ID = parseInt($(this).attr("id"));        
        $.get("/Machines/GetProgramMachineDetails", { ID }, function (msg) {

            if (msg != '') {

                data = JSON.parse(msg);
                if (data != null) {
                    GetPlantDetails(data.PlantID, data.Shopid, data.CellId);
                    //GetshopDetails(data.Shopid);
                    //GetCellDetails(data.CellId);
                    $("#EditMachineInvNo").val(data.MachineInvNo);
                    $("#EditControllerType").val(data.ControllerType);
                    $("#EditMachineDispName").val(data.MachineDispName);
                    $("#EditMachineMake").val(data.MachineMake);
                    $("#EditMachineModel").val(data.MachineModel);
                    $("#EditMachineModel").val(data.MachineModel);
                    GetProgramDetails(data.ProgramType);
                    if (data.ProgramType == 1) {
                        $('.lblip').show();
                        $('.DivIPAddress').show();
                        $("#Editipaddress").val(data.IpAddress);

                        $('.lblusr').attr('display', 'none');
                        $('.DivUsername').attr('display', 'none');
                        $('.lblpwd').attr('display', 'none');
                        $('.DivPwd').attr('display', 'none');

                        $('.lblusr').hide();
                        $('.DivUsername').hide();
                        $('.lblpwd').hide();
                        $('.DivPwd').hide();

                        $('.lblport').hide();
                        $('.DivPort').hide();
                        $('.lbldmn').hide();
                        $('.DivDmn').hide();
                    }
                    else if (data.ProgramType == 2)
                    {
                        $('.lblip').show();
                        $('.DivIPAddress').show();
                        $('.lblusr').show();
                        $('.DivUsername').show();
                        $('.lblpwd').show();
                        $('.DivPwd').show();

                        $('.lblport').hide();
                        $('.DivPort').hide();
                        $('.lbldmn').hide();
                        $('.DivDmn').hide();


                        $("#Editipaddress").val(data.IpAddress);
                        $("#EditUserName").val(data.UserName);
                        $("#EditPwd").val(data.Password);
                        $("#EditPort").val(data.Port);
                        $("#EditDomain").val(data.Domain);
                        
                    }
                    else if (data.ProgramType == 3) {
                        $('.lblip').show();
                        $('.DivIPAddress').show();
                        $('.lblusr').show();
                        $('.DivUsername').show();
                        $('.lblpwd').show();
                        $('.DivPwd').show();

                        $('.lblport').show();
                        $('.DivPort').show();
                        $('.lbldmn').show();
                        $('.DivDmn').show();

                        $("#Editipaddress").val(data.IpAddress);
                        $("#EditUserName").val(data.UserName);
                        $("#EditPwd").val(data.Password);
                        $("#EditPort").val(data.Port);
                        $("#EditDomain").val(data.Domain);
                    }
                }
              
               

                //$("#EditProgramType").val();
                //if (programtype == 1) {
                //    ipaddress = $("#ipaddress").val();
                //}
                //else if (programtype == 2) {
                //    ipaddress = $("#ipaddress").val();
                //    username = $("#Username").val();
                //    password = $("#password").val();
                //}
                //else if (programtype == 3) {
                //    ipaddress = $("#ipaddress").val();
                //    username = $("#Username").val();
                //    password = $("#password").val();
                //    port = $("#port").val();
                //    domain = $("#domain").val();
                //}
            }
        });

    });

    $(document).on("change", "#EditProgramType", function (e) {
        //$("#ProgramType").on("change", function () {
        var id = this;
        //var prog = this.val();
        var progtype = $("#EditProgramType").val();
       
        if (progtype == "1") {

            $('.lblip').show();
            $('.DivIPAddress').show();
            

            $('.lblusr').attr('display', 'none');
            $('.DivUsername').attr('display', 'none');
            $('.lblpwd').attr('display', 'none');
            $('.DivPwd').attr('display', 'none');

            $('.lblusr').hide();
            $('.DivUsername').hide();
            $('.lblpwd').hide();
            $('.DivPwd').hide();

            $('.lblport').hide();
            $('.DivPort').hide();
            $('.lbldmn').hide();
            $('.DivDmn').hide();

        }

        else if (progtype == "2") {
            $('.lblip').show();
            $('.DivIPAddress').show();
            $('.lblusr').show();
            $('.DivUsername').show();
            $('.lblpwd').show();
            $('.DivPwd').show();

            $('.lblport').hide();
            $('.DivPort').hide();
            $('.lbldmn').hide();
            $('.DivDmn').hide();
        }

        else if (progtype == "3") {
            $('.lblip').show();
            $('.DivIPAddress').show();
            $('.lblusr').show();
            $('.DivUsername').show();
            $('.lblpwd').show();
            $('.DivPwd').show();

            $('.lblport').show();
            $('.DivPort').show();
            $('.lbldmn').show();
            $('.DivDmn').show();

        }
      

       

    });

});
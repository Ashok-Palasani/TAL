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
        
        var progtype = $("#ProgramType").val();
        var cssdata = "";
      
        if (progtype == "1") {
           
            $('#divip').show();
            $('#divPath').show();          
            $('#divPort').hide();
            $('#divUsr').hide();
            $('#divpassword').hide();
            $('#divDomain').hide();
          
        }

        else if (progtype == "2") {
            //FTP 
            $('#divip').show();
            $('#divPort').show();
            $('#divUsr').show();
            $('#divpassword').show();
            $('#divPath').show();            
            $('#divDomain').hide();
        }

        else if (progtype == "3") {
            $('#divip').show();
            $('#divPort').show();
            $('#divUsr').show();
            $('#divpassword').show();
            $('#divPath').show();           
            $('#divDomain').show();
        }
        else
        {
            $('#divip').hide();
            $('#divPath').hide();
            $('#divPort').hide();
            $('#divUsr').hide();
            $('#divpassword').hide();
            $('#divDomain').hide();
        }
       
       

    });

    //Add Machine details
    $(document).on("click", "#savebtn", function (e) {
        var ipaddress = "";
        var username = "";
        var password = "";
        var port = 0;
        var domain = "";
        var ProgramPath = '';
        var plantid = parseInt($("#plantID").val());
        var shopid = parseInt($("#DepartmentID").val());
        var cellid = parseInt($("#MachineCategoryID").val());
        var MachineInvNo = $("#MachineInvNo").val();
        var MachineModel = $("#MachineModel").val();
        var ControllerType = $("#ControllerType").val();
        var MachineDispName = $("#MachineDispName").val();
        var MachineMake = $("#MachineMake").val();
        var portval = $("#port").val();
        var programtype = $("#ProgramType").val();
        if (programtype == 1) {
            ipaddress = $("#ipaddress").val();
            ProgramPath = $("#PathName").val();
            portval = 'no port';
        }
        else if (programtype == 2) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
            if (portval != '')
                port = parseInt(portval);
            ProgramPath = $("#PathName").val();
        }
        else if (programtype == 3) {
            ipaddress = $("#ipaddress").val();
            username = $("#Username").val();
            password = $("#password").val();
            if (portval != '')
                port = parseInt(portval);
            domain = $("#domain").val();
        }
        if (portval != 0 && MachineInvNo != '' && programtype != 0 && plantid != 0 && shopid != 0 && cellid != 0) {
            $.post("/Machines/CreateData", { plantid, shopid, cellid, MachineInvNo, MachineModel, ControllerType, MachineDispName, MachineMake, programtype, ipaddress, username, password, port, domain, ProgramPath }, function (msg) {
                if (msg == "Success") {
                    alert("Machine details Created Successfully");
                    window.location.href = "/Machines/MachineList";
                }

            });
        }
        else
        {
            alert("Please enter all the Fields");
        }
    });

    //Update Machine Details
    $(document).on("click", "#btnupdate", function (e) {
        var ipaddress = "";
        var username = "";
        var password = "";
        var port = 0;
        var domain = "";
        
        var id = parseInt($("#hdntxt").val());      
        var plantid =parseInt( $("#EditPlantID").val());
        var shopid = parseInt($("#EditShopID").val());
        var cellid = parseInt($("#EditCellID").val());
        var MachineInvNo = $("#EditMachineInvNo").val();
        var ControllerType = $("#EditControllerType").val();
        var MachineDispName = $("#EditMachineDispName").val();
        var MachineMake = $("#EditMachineMake").val();
        var MachineModel = $("#EditMachineModel").val();
        var ProgramPath = $('#EditPathName').val();
        var programtype = parseInt($("#EditProgramType").val());
        var portval = $("#EditPort").val();
        if (programtype == 1) {
            ipaddress = $("#Editipaddress").val();
            portval = 'no port';
        }
        else if (programtype == 2) {
            ipaddress = $("#Editipaddress").val();
            username = $("#EditUserName").val();
            password = $("#EditPwd").val();
            if (portval != '')
                port = parseInt(portval);
        }
        else if (programtype == 3) {
            ipaddress = $("#Editipaddress").val();
            username = $("#EditUserName").val();
            password = $("#EditPwd").val();
            if (portval != '')
                port = parseInt(portval);
            domain = $("#EditDomain").val();
        }
        if (portval !='' && MachineInvNo != '' && programtype != 0 && plantid != 0 && shopid != 0 && cellid != 0) {
            $.post("/Machines/EditData", { id, plantid, shopid, cellid, MachineInvNo, ControllerType, MachineDispName, MachineMake, MachineModel, programtype, ipaddress, username, password, port, domain, ProgramPath }, function (data) {
                if (data == "Success") {
                    alert("Machine Updated Successfully");
                    window.location.href = "/Machines/MachineList";
                }

            });
        }
        else
        {
            alert("Please enter all the Fields");

        }
    });

    //Edit Machinedetails
    $(document).on("click", '.MachineEdit', function (e) {
        var ID = parseInt($(this).attr("id"));        
        $.get("/Machines/GetProgramMachineDetails", { ID }, function (msg) {

            if (msg != '') {

                data = JSON.parse(msg);
                if (data != null) {
                    $("#hdntxt").val(ID);
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
                      
                        $('#DivIPAddress').show();
                        $("#Editipaddress").val(data.IpAddress); 
                        $('#DivPath').show(); 
                        $('#EditPathName').val(data.MachineProgramPath);

                        $('#DivUsername').hide();                        
                        $('#DivPwd').hide();                    
                        $('#DivPort').hide();                        
                        $('#DivDmn').hide();
                    }
                    else if (data.ProgramType == 2)
                    {
                       
                        $('#DivIPAddress').show();                       
                        $('#DivUsername').show();                        
                        $('#DivPwd').show();
                        $('#DivPath').show();
                        
                        $('#DivPort').show();                      
                        $('#DivDmn').hide();

                        $('#EditPathName').val(data.MachineProgramPath);
                        $("#Editipaddress").val(data.IpAddress);
                        $("#EditUserName").val(data.UserName);
                        $("#EditPwd").val(data.Password);
                        $("#EditPort").val(data.Port);
                        $("#EditDomain").val(data.Domain);
                        
                    }
                    else if (data.ProgramType == 3) {
                      
                        $('#DivIPAddress').show();                       
                        $('#DivUsername').show();                        
                        $('#DivPwd').show();
                        $('#DivPort').show();                        
                        $('#DivDmn').show();
                        $('#DivPath').show();

                        $("#Editipaddress").val(data.IpAddress);
                        $("#EditUserName").val(data.UserName);
                        $("#EditPwd").val(data.Password);
                        $("#EditPort").val(data.Port);
                        $("#EditDomain").val(data.Domain);
                        $('#EditPathName').val(data.MachineProgramPath);
                    }
                   
                }              
            }
        });

    });

    $(document).on("change", "#EditProgramType", function (e) {
        //$("#ProgramType").on("change", function () {
        var id = this;
        //var prog = this.val();
        var progtype = $("#EditProgramType").val();
       
        if (progtype == "1") {            
            $('#DivIPAddress').show();
            $('#DivPath').show();
            $('#DivUsername').hide();
            $('#DivPwd').hide();
            $('#DivPort').hide();
            $('#DivDmn').hide();
        }

        else if (progtype == "2") {
            $('#DivIPAddress').show();
            $('#DivUsername').show();
            $('#DivPwd').show();
            $('#DivPath').show();

            $('#DivPort').show();
            $('#DivDmn').hide();
        }

        else if (progtype == "3") {
            $('#DivIPAddress').show();
            $('#DivUsername').show();
            $('#DivPwd').show();
            $('#DivPort').show();
            $('#DivDmn').show();
            $('#DivPath').show();
        }
        else {
            $('#DivPath').hide();
            $('#DivIPAddress').hide();
            $('#DivUsername').hide();
            $('#DivPwd').hide();
            $('#DivPort').hide();
            $('#DivDmn').hide();
        } 
    });

    $(document).on('click', '.btnContinueDelete', function (e) {
        var id =parseInt( $("#hdntxt").val());
        $.post("/Machines/DeleteMachine", { id }, function (msg) {
            if (msg != '') {
                alert("Machine Deleted Successfully");
                window.location.href = "/Machines/MachineList";
            }
        });
    });

    $(document).on('click', '.Machinedelete', function (e) {
        var ID = parseInt($(this).attr("id"));
        $("#hdntxt").val(ID);
    });

});
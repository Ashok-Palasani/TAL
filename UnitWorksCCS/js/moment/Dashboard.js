$(document).ready(function () {
    pageload();
    GetChart1();
    GetChart2();
    setInterval(pageload, 1000);
    setInterval(GetChart1, 60*1000);
    //var machineid = 0;
    function pageload() {
        $.get("/Gentella/GetMachine", {}, function (msg) {
            $('.MACDET').empty();
            if (msg !== '') {             
                var data = JSON.parse(msg);
                var cssdata = '';
                for (var i = 0; i < data.length && i < 4; i++) {
                    var color = '';
                    if (data[i].Color === 'GREEN')
                        color = 'dash-green';
                    else if (data[i].Color === 'YELLOW')
                        color = 'dash-amber';
                    else if (data[i].Color === 'BLUE')
                        color = 'dash-blue';
                    else if (data[i].Color === 'RED')
                        color = 'dash-red';
                    cssdata += '<div class="col-sm-6"><div class="dash-img-border ' + color + '">';
                    cssdata += '<div class="row"><div class="col-sm-6"><div style="padding: 0 0px 0 19px;"><span class="dash-title">' + data[i].MachineName + '</span></div><img src="/images/CNC_MACHINE.png" class="img-responsive dash-img"></div>';
                    cssdata += '<div class="col-sm-6"> <div class="dash-dasc-pad text-center"><span class="dash-desc">Plant - Shop - Cell</span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right">Power on Time: </span> <span class="col-sm-5 dash-desc">' + data[i].PowerOnTime + '</span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right">Running Time: </span><span class="col-sm-5 dash-desc">' + data[i].RunningTime + ' </span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right"> Cutting Time: </span><span class="col-sm-5 dash-desc">' + data[i].CuttingTime + ' </span ></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right"> Total Parts Count: </span ><span class="col-sm-5 dash-desc">' + data[i].PartsCount + ' </span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right"> Current Status: </span ><span class="col-sm-5 dash-desc">' + data[i].CurrentStatus + ' </span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right"> Exe Program Name: </span ><span class="col-sm-5 dash-desc">' + data[i].Program + ' </span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" > Cycle Time: </span><span class="col-sm-5 dash-desc">' + data[i].CycleTime + ' </span></div>';                  
                    cssdata += '</div></div></div></div>';
                }
               
                //cssdata += '</div>';
                $(cssdata).appendTo('.MACDET');
            }
        });
    }

    var chart1 = AmCharts.makeChart("chartdiv",
        {
            "type": "serial",
            "categoryField": "MachineName",
            "angle": 30,
            "depth3D": 30,
            "startDuration": 1,
            "categoryAxis": {
                "gridPosition": "start"
            },
            "trendLines": [],
            "graphs": [
                {
                    "balloonText": "[[title]] of [[MachineName]]:[[value] ] hrs",
                    "fillAlphas": 1,
                    "id": "AmGraph-1",
                    "title": "Breakdown Time",
                    "type": "column",
                    "valueField": "BreakDownTime"


                },
                {
                    "balloonText": "[[title]] of [[MachineName]]:[[value]]  hrs",
                    "fillAlphas": 1,
                    "id": "AmGraph-2",
                    "title": "Idle Time",
                    "type": "column",
                    "valueField": "IdleTime"

                },
                {
                    "balloonText": "[[title]] of [[MachineName]]:[[value] ]  hrs",
                    "fillAlphas": 1,
                    "id": "AmGraph-3",
                    "title": "Running Time", 
                    "type": "column",
                    "valueField": "RunningTime"
                  
                },

               
               
                {
                    "balloonText": "[[title]] of [[MachineName]]:[[value]]  hrs",
                    "fillAlphas": 1,
                    "id": "AmGraph-4",
                    "title": "Power Off",
                    "type": "column",
                    "valueField": "PowerOffTime"
                  
                }
            ],
            "guides": [],
            "valueAxes": [
                {
                    "id": "ValueAxis-1",
                    "stackType": "3d",
                    "title": "Time in (Hrs)"
                }
            ],
            "allLabels": [],
            "balloon": {},
            "legend": {
                "enabled": true,
                "useGraphSettings": true
            }
          
        }
    );

    var chart = AmCharts.makeChart("chartdiv1", {
        "type": "serial",
        "theme": "light",
        "marginTop": 0,
        "marginRight": 0,
        //"dataProvider": [{
        //    "Time": "00:10:00",
        //    "value": 0.1
        //}, {
        //    "Time": "03:20:00",
        //    "value": 0.12
        //}, {
        //    "Time": "20:30:10",
        //    "value": 0.073
        //}, {
        //    "Time": "1953",
        //    "value": -0.027
        //}, {
        //    "Time": "1954",
        //    "value": -0.251
        //}, {
        //    "Time": "1955",
        //    "value": -0.281
        //}, {
        //    "Time": "1956",
        //    "value": -0.348
        //}, {
        //    "Time": "1957",
        //    "value": -0.074
        //}, {
        //    "Time": "1958",
        //    "value": -0.011
        //}, {
        //    "Time": "1959",
        //    "value": -0.074
        //}, {
        //    "Time": "1960",
        //    "value": -0.124
        //}, {
        //    "Time": "1961",
        //    "value": -0.024
        //}, {
        //    "Time": "1962",
        //    "value": -0.022
        //}, {
        //    "Time": "1963",
        //    "value": 0
        //}, {
        //    "Time": "1964",
        //    "value": -0.296
        //}, {
        //    "Time": "1965",
        //    "value": -0.217
        //}, {
        //    "Time": "1966",
        //    "value": -0.147
        //}, {
        //    "Time": "1967",
        //    "value": -0.15
        //}, {
        //    "Time": "1968",
        //    "value": -0.16
        //}, {
        //    "Time": "1969",
        //    "value": -0.011
        //}, {
        //    "Time": "1970",
        //    "value": -0.068
        //}, {
        //    "Time": "1971",
        //    "value": -0.19
        //}, {
        //    "Time": "1972",
        //    "value": -0.056
        //}, {
        //    "Time": "1973",
        //    "value": 0.077
        //}, {
        //    "Time": "1974",
        //    "value": -0.213
        //}, {
        //    "Time": "1975",
        //    "value": -0.17
        //}, {
        //    "Time": "1976",
        //    "value": -0.254
        //}, {
        //    "Time": "1977",
        //    "value": 0.019
        //}, {
        //    "Time": "1978",
        //    "value": -0.063
        //}, {
        //    "Time": "1979",
        //    "value": 0.05
        //}, {
        //    "Time": "1980",
        //    "value": 0.077
        //}, {
        //    "Time": "1981",
        //    "value": 0.12
        //}, {
        //    "Time": "1982",
        //    "value": 0.011
        //}, {
        //    "Time": "1983",
        //    "value": 0.177
        //}, {
        //    "Time": "1984",
        //    "value": -0.021
        //}, {
        //    "Time": "1985",
        //    "value": -0.037
        //}, {
        //    "Time": "1986",
        //    "value": 0.03
        //}, {
        //    "Time": "1987",
        //    "value": 0.179
        //}, {
        //    "Time": "1988",
        //    "value": 0.18
        //}, {
        //    "Time": "1989",
        //    "value": 0.104
        //}, {
        //    "Time": "1990",
        //    "value": 0.255
        //}, {
        //    "Time": "1991",
        //    "value": 0.21
        //}, {
        //    "Time": "1992",
        //    "value": 0.065
        //}, {
        //    "Time": "1993",
        //    "value": 0.11
        //}, {
        //    "Time": "1994",
        //    "value": 0.172
        //}, {
        //    "Time": "1995",
        //    "value": 0.269
        //}, {
        //    "Time": "1996",
        //    "value": 0.141
        //}, {
        //    "Time": "1997",
        //    "value": 0.353
        //}, {
        //    "Time": "1998",
        //    "value": 0.548
        //}, {
        //    "Time": "1999",
        //    "value": 0.298
        //}, {
        //    "Time": "2000",
        //    "value": 0.267
        //}, {
        //    "Time": "2001",
        //    "value": 0.411
        //}, {
        //    "Time": "2002",
        //    "value": 0.462
        //}, {
        //    "Time": "2003",
        //    "value": 0.47
        //}, {
        //    "Time": "2004",
        //    "value": 0.445
        //}, {
        //    "Time": "2005",
        //    "value": 0.47
        //}
        //],
        "valueAxes": [{
            "axisAlpha": 0,
            "position": "left",
            "guides": [{
                "value": 0.05,
                "lineAlpha": 1,
                "lineColor": "#880000"
            }, {
                "value": -0.15,
                "lineAlpha": 1,
                "lineColor": "#880088"
            }],
            "axisAlpha": 0,
            "zeroGridAlpha": 0
        }],
        "graphs": [{
            "balloonText": "<div style='margin:5px; font-size:19px;'><span style='font-size:13px;'>[[ca tegory]]</span><br>[[value]]</div>",
            "bullet": "round",
            "bulletSize": 8,
            "bulletBorderAlpha": 0,
            "lineThickness": 2,
            "negativeLineColor": "#FFC107",
            "negativeBase": 0.05,
            "type": "smoothedLine",
            "valueField": "value"
        }, {
            "showBalloon": false,
            "bullet": "round",
            "bulletBorderAlpha": 0,
            "hideBulletsCount": 50,
            "lineColor": "transparent",
            "negativeLineColor": "#D50000",
            "negativeBase": -0.1,
            "type": "smoothedLine",
            "valueField": "value"
        }],
        "chartScrollbar": {
            "graph": "g1",
            "gridAlpha": 0,
            "color": "#888888",
            "scrollbarHeight": 55,
            "backgroundAlpha": 0,
            "selectedBackgroundAlpha": 0.1,
            "selectedBackgroundColor": "#888888",
            "graphFillAlpha": 0,
            "autoGridCount": true,
            "selectedGraphFillAlpha": 0,
            "graphLineAlpha": 0.2,
            "graphLineColor": "#c2c2c2",
            "selectedGraphLineColor": "#888888",
            "selectedGraphLineAlpha": 1

        },
        "chartCursor": {
            "categoryBalloonDateFormat": "hh:mm:ss",
            "cursorAlpha": 0,
            "valueLineEnabled": false,
            "valueLineBalloonEnabled": false,
            "valueLineAlpha": 0.5,
            "fullWidth": true
        },
        "dataDateFormat": "HH:mm:ss",
        "categoryField": "Time",
        "categoryAxis": {
            "minPeriod": "mm",
            "parseDates": true,
            "minorGridAlpha": 0.1,
            "minorGridEnabled": true
        }
    });

    chart.addListener("rendered", zoomChart);
    if (chart.zoomChart) {
        chart.zoomChart();
    }

    function zoomChart() {
        chart.zoomToIndexes(Math.round(chart.dataProvider.length * 0.4), Math.round(chart.dataProvider.length * 0.55));
    }
    

    function GetChart1() {
        $.get('/Gentella/GetAxisDetails', {}, function (msg) {
           
            if (msg !== '') {
                //var dat = msg;

                chart1.dataProvider = JSON.parse(msg);
                chart1.validateData();
            }
        });
    }



    function GetChart2() {
        $.get('/Gentella/GetSpindleLoad', {}, function (msg) {

            if (msg !== '') {
                //var dat = msg;

                chart.dataProvider = JSON.parse(msg);
                chart.validateData();
            }
        });
    }
});
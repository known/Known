var KBMap = {
    isLoad: false,
    ak: 'stivIs1QN5tRtrmycPDBKREhfGDigZKN',
    url: 'https://api.map.baidu.com/api?v=3.0&ak=',
    id: '',
    option: {},
    style: {
        dark: {
            styleJson: [
                {
                    "featureType": "water",
                    "elementType": "all",
                    "stylers": {
                        "color": "#021019"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "geometry.stroke",
                    "stylers": {
                        "color": "#147a92"
                    }
                },
                {
                    "featureType": "arterial",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "arterial",
                    "elementType": "geometry.stroke",
                    "stylers": {
                        "color": "#0b3d51"
                    }
                },
                {
                    "featureType": "local",
                    "elementType": "geometry",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "land",
                    "elementType": "all",
                    "stylers": {
                        "color": "#08304b"
                    }
                },
                {
                    "featureType": "railway",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "railway",
                    "elementType": "geometry.stroke",
                    "stylers": {
                        "color": "#08304b"
                    }
                },
                {
                    "featureType": "subway",
                    "elementType": "geometry",
                    "stylers": {
                        "lightness": -70
                    }
                },
                {
                    "featureType": "building",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "all",
                    "elementType": "labels.text.fill",
                    "stylers": {
                        "color": "#857f7f"
                    }
                },
                {
                    "featureType": "all",
                    "elementType": "labels.text.stroke",
                    "stylers": {
                        "color": "#000000"
                    }
                },
                {
                    "featureType": "building",
                    "elementType": "geometry",
                    "stylers": {
                        "color": "#022338"
                    }
                },
                {
                    "featureType": "green",
                    "elementType": "geometry",
                    "stylers": {
                        "color": "#062032"
                    }
                },
                {
                    "featureType": "boundary",
                    "elementType": "all",
                    "stylers": {
                        "color": "#1e1c1c"
                    }
                },
                {
                    "featureType": "manmade",
                    "elementType": "geometry",
                    "stylers": {
                        "color": "#022338"
                    }
                },
                {
                    "featureType": "poi",
                    "elementType": "all",
                    "stylers": {
                        "visibility": "off"
                    }
                },
                {
                    "featureType": "all",
                    "elementType": "labels.icon",
                    "stylers": {
                        "visibility": "off"
                    }
                },
                {
                    "featureType": "all",
                    "elementType": "labels.text.fill",
                    "stylers": {
                        "color": "#2da0c6",
                        "visibility": "on"
                    }
                }
            ]
        },
        light: {
            styleJson: [
                {
                    "featureType": "land",
                    "elementType": "geometry",
                    "stylers": {
                        "color": "#e7f7fc"
                    }
                },
                {
                    "featureType": "water",
                    "elementType": "all",
                    "stylers": {
                        "color": "#96b5d6"
                    }
                },
                {
                    "featureType": "green",
                    "elementType": "all",
                    "stylers": {
                        "color": "#b0d3dd"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#a6cfcf"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "geometry.stroke",
                    "stylers": {
                        "color": "#7dabb3"
                    }
                },
                {
                    "featureType": "arterial",
                    "elementType": "geometry.fill",
                    "stylers": {
                        "color": "#e7f7fc"
                    }
                },
                {
                    "featureType": "arterial",
                    "elementType": "geometry.stroke",
                    "stylers": {
                        "color": "#b0d5d4"
                    }
                },
                {
                    "featureType": "local",
                    "elementType": "labels.text.fill",
                    "stylers": {
                        "color": "#7a959a"
                    }
                },
                {
                    "featureType": "local",
                    "elementType": "labels.text.stroke",
                    "stylers": {
                        "color": "#d6e4e5"
                    }
                },
                {
                    "featureType": "arterial",
                    "elementType": "labels.text.fill",
                    "stylers": {
                        "color": "#374a46"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "labels.text.fill",
                    "stylers": {
                        "color": "#374a46"
                    }
                },
                {
                    "featureType": "highway",
                    "elementType": "labels.text.stroke",
                    "stylers": {
                        "color": "#e9eeed"
                    }
                }
            ]
        }
    },
    load: function () {
        if (!this.isLoad) {
            var src = this.url + this.ak + '&callback=KBMap.initMap';
            //window.onload = function () {
            var script = document.createElement("script");
            script.src = src;
            document.body.appendChild(script);
            //}
            this.isLoad = true;
        }
    },
    init: function (id, option) {
        this.id = id;
        this.option = option;
        if (!this.isLoad) {
            this.load();
        } else {
            this.initMap();
        }
    },
    setMap: function (id, option) {
        this._initMap(id, option);
    },
    initMap: function () {
        if (!this.id)
            return;

        this._initMap(this.id, this.option);
    },
    _initMap: function (id, option) {
        var map = new BMap.Map(id);
        map.setMapStyle(this.style[option.style || 'light']);
        option.callback && option.callback(map);
    }
};

KBMap.createSelectMapButton = function (e) {
    _createMapButton(e.form, 'mapContainer').appendTo(e.parent);
    KBMap.load();

    function _createMapButton(form, containerId) {
        return $('<button>').html(Language.Select).click(function () {
            var map, dlg, input;
            dlg = Layer.open({
                title: Language.SelectCoordinate, width: 650, height: 350,
                content: function (body) {
                    var elemQ = $('<div>').appendTo(body);
                    input = $('<input>')
                        .attr('type', 'text')
                        .attr('id', 'mapKey')
                        .css({ width: '200px', marginRight: '2px' })
                        .appendTo(elemQ);
                    $('<button>').html(Language.Query).appendTo(elemQ).on('click', function () {
                        var local = new BMap.LocalSearch(map, {
                            renderOptions: { map: map }
                        });
                        var key = document.getElementById('mapKey').value;
                        local.search(key);
                    });
                    $('<div>').attr('id', containerId)
                        .addClass('fit')
                        .css({ top: '40px', zIndex: '99999' })
                        .appendTo(body);
                },
                success: function () {
                    map = new BMap.Map(containerId, { enableMapClick: false });
                    map.enableScrollWheelZoom(true);
                    map.enableContinuousZoom();
                    map.setDefaultCursor('default');
                    map.setDraggingCursor('default');
                    map.addEventListener("click", function (e) {
                        map.clearOverlays();
                        form.Longitude.setValue(e.point.lng);
                        form.Latitude.setValue(e.point.lat);
                        //_setMapPoint(map, e.point.lng, e.point.lat);
                        dlg.close();
                    });
                    var lng = form.Longitude.getValue();
                    var lat = form.Latitude.getValue();
                    var province = form.Province ? form.Province.getValue() : '';
                    var city = form.City ? form.City.getValue() : '';
                    var area = form.Area ? form.Area.getValue() : '';
                    if (lng && lat) {
                        _setMapPoint(map, lng, lat);
                    } else if (area) {
                        input.val(area);
                        map.centerAndZoom(area, 13);
                    } else if (city) {
                        input.val(city);
                        map.centerAndZoom(city, 10);
                    } else if (province) {
                        input.val(province);
                        map.centerAndZoom(province, 8);
                    }
                }
            });
        });
    }

    function _setMapPoint(map, lng, lat) {
        var point = new BMap.Point(lng, lat);
        map.centerAndZoom(point, 15);
        var marker = new BMap.Marker(point);
        map.addOverlay(marker);
    }
}

KBMap.setBoundary = function (map, city) {
    var bdary = new BMap.Boundary();
    bdary.get(city, function (rs) {
        var count = rs.boundaries.length;
        if (count === 0) {
            alert(Language.NoCurrentAreaTips);
            return;
        }
        var pointArray = [];
        for (var i = 0; i < count; i++) {
            var ply = new BMap.Polygon(rs.boundaries[i], {
                strokeWeight: 2, strokeColor: "#014F99", fillColor: "#DDE4F0"
            });
            map.addOverlay(ply);
            pointArray = pointArray.concat(ply.getPath());
        }
        map.setViewport(pointArray);
    });
}

KBMap.addMapMarker = function (map, item, onClick) {
    var point = new BMap.Point(item.Longitude, item.Latitude);
    var marker = new BMap.Marker(point);
    map.addOverlay(marker);
    marker.setTitle(item.Name);
    marker.addEventListener('click', function (e) {
        //var av = e.currentTarget.Av;
        //for (var i = 0; i < data.length; i++) {
        //    if (data[i].lng === av.lng && data[i].lat === av.lat) {
        //    }
        //}
        //var opts = { width: 100, height: 30, title: '水厂信息' };
        //var infoWindow = new BMap.InfoWindow(name, opts);
        //map.openInfoWindow(infoWindow, e.point);
        onClick && onClick(item);
    });
}
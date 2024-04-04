var map = null;
var containerId = null;

function initMapsG(dotnetRef) {
    initMaps(containerId);
    geoLocation(dotnetRef);
}

function initMaps(elementId) {
    map = new BMap.Map(elementId, {
        //coordsType指定输入输出的坐标类型，3为gcj02坐标，5为bd0ll坐标，默认为5。
        //指定完成后API将以指定的坐标类型处理您传入的坐标
        coordsType: 5
    });
    //创建默认地点坐标，北京
    var point = new BMap.Point(116.47496, 39.77856);
    map.centerAndZoom(point, 15);
    map.enableScrollWheelZoom(true);
    map.addControl(new BMap.NavigationControl());
    map.addControl(new BMap.ScaleControl());
    map.addControl(new BMap.OverviewMapControl());
    //map.addControl(new BMap.MapTypeControl());
    //map.setCurrentCity("北京");
}

export function addScript(key, elementId, dotnetRef) {
    if (!key || !elementId)
        return;

    containerId = elementId;
    let url = "https://api.map.baidu.com/api?v=3.0&ak=";
    let scriptsIncluded = false;

    let scriptTags = document.querySelectorAll('body > script');
    scriptTags.forEach(scriptTag => {
        if (scriptTag) {
            let srcAttribute = scriptTag.getAttribute('src');
            if (srcAttribute && srcAttribute.startsWith(url)) {
                scriptsIncluded = true;
                return true;
            }
        }
    });

    if (scriptsIncluded) {
        initMapsG(dotnetRef);
        return true;
    }

    url = url + key + "&callback=initMapsG";
    let script = document.createElement('script');
    script.src = url;
    document.body.appendChild(script);
    return false;
}

export function resetMaps() {
    initMaps(containerId);
}

export function searchLocation(name, dotnetRef) {
    var options = {
        renderOptions: { map: map },
        onSearchComplete: function (results) {
            if (local.getStatus() == BMAP_STATUS_SUCCESS) {
                //console.log(results);
                var pois = [];
                for (var i = 0; i < results.getCurrentNumPois(); i++) {
                    //console.log(results.getPoi(i));
                    pois.push(results.getPoi(i));
                }
                if (dotnetRef) {
                    dotnetRef.invokeMethodAsync('GetSearch', pois);
                }
            }
        }
    };
    var local = new BMap.LocalSearch(map, options);
    local.search(name);
}

export function geoLocation(wrapper) {
    var geolocation = new BMap.Geolocation();
    geolocation.enableSDKLocation();
    geolocation.getCurrentPosition(function (r) {
        let location;
        if (this.getStatus() == BMAP_STATUS_SUCCESS) {
            var mk = new BMap.Marker(r.point);
            map.addOverlay(mk);
            map.panTo(r.point);
            location = r;
            location.Status = BMAP_STATUS_SUCCESS;//0
            //console.log(r);
            map.centerAndZoom(location.point, 15);
            //map.setCurrentCity(location.address.city);
        }
        else {
            location = { "Status": this.getStatus() };
        }

        if (wrapper) {
            wrapper.invokeMethodAsync('GetLocation', location);
        }

        return location;
    });
}
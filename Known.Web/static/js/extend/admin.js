layui.define("common",function(n){function o(){var n=document.documentElement,i=n.requestFullScreen||n.webkitRequestFullScreen||n.mozRequestFullScreen||n.msRequestFullScreen,t;i?i.call(n):typeof ActiveXObject!="undefined"&&(t=new ActiveXObject("WScript.Shell"),t!==null&&t.SendKeys("{F11}"))}function s(){var n=document,i=n.cancelFullScreen||n.webkitCancelFullScreen||n.mozCancelFullScreen||n.exitFullScreen,t;i?i.call(n):typeof ActiveXObject!="undefined"&&(t=new ActiveXObject("WScript.Shell"),t!==null&&t.SendKeys("{F11}"))}function h(n){n&&t("head").append("<style>\n.layui-tab-item.layui-show{animation:moveTop 1s;-webkit-animation:moveTop 1s;animation-fill-mode:both;-webkit-animation-fill-mode:both;position:relative;height:100%;-webkit-overflow-scrolling:touch;}\n@keyframes moveTop {\n  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n}\n@-o-keyframes moveTop {\n  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n}\n@-moz-keyframes moveTop {\n  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n}\n@-webkit-keyframes moveTop {\n  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n}\n<\/style>")}var t=layui.$,f=layui.layer,i=layui.element,e=layui.common,c={option:null,treeData:null,show:function(n){var i,r;this.option=n;this.treeData=e.list2Tree(n.data,"");i=this.render("topMenu");this.render("leftMenu",i[0].id);this.initEvent();r=t(".layui-nav[lay-filter=topMenu]").hide();i.length>1&&r.show();n.topChanged&&n.topChanged(i[0])},render:function(n,r){function o(n){if(n.target==="_blank")return'<a href="'+n.url+'" id="menu'+n.id+'" target="_blank"><i class="layui-icon '+n.icon+'"><\/i>';var t=n.url?' data-url="'+n.url+'"':"";return'<a href="javascript:;" id="menu'+n.id+'"'+t+'><i class="layui-icon '+n.icon+'"><\/i>'}var u="",f=this.treeData,e;return r&&(e=this.treeData.find(n=>n.id===r),f=e.children),t(f).each(function(n,i){u+='<li class="layui-nav-item menuItem">';u+=o(i)+'<span class="title">'+i.title+"<\/span><\/a>";r&&i.children&&(u+='  <dl class="layui-nav-child">',t(i.children).each(function(n,t){u+="<dd>"+o(t)+" "+t.title+"<\/a><\/dd>"}),u+="  <\/dl>");u+="<\/li>"}),t(".layui-nav[lay-filter="+n+"]").html(u),i.init(),f},miniSide:!1,initEvent:function(){var n=this;t(".toggleMenu").click(function(){var r="layui-icon-spread-left",u="layui-icon-shrink-right";n.miniSide?(n.miniSide=!1,t(this).removeClass(r).addClass(u),t(".layui-layout-admin").removeClass("layui-mini")):(n.miniSide=!0,t(this).removeClass(u).addClass(r),t(".layui-layout-admin").addClass("layui-mini"));n.initMenuTips();i.init()});n.initMenuEvent();i.on("nav(topMenu)",function(t){var i=t[0].id.replace("menu",""),r=t.data("url");n.render("leftMenu",i);n.initMenuTips();n.initMenuEvent();n.option.topChanged&&n.option.topChanged({url:r})})},initMenuEvent:function(){var n=this;t(".layui-nav-tree [data-url]").click(function(){var i,u,f;n.closeMenuTips();i=t(this);u=i.data("url");u&&(f=i[0],r.addTab({id:f.id.replace("menu",""),text:f.text,icon:i.find("i")[0].outerHTML,url:u}))})},menuTipId:"",initMenuTips:function(){var i=t(".layui-side .menuItem").unbind("mouseenter"),r=t(".popup-tips").unbind("mouseleave"),n;(this.closeMenuTips(),this.miniSide)&&(n=this,i.bind("mouseenter",function(){var i=t(this).html();i='<ul class="layui-nav layui-nav-tree layui-this"><li class="layui-nav-item popMenuItem layui-nav-itemed">'+i+"<\/li><\/ul>";n.menuTipId=f.tips(i,t(this),{tips:[2,"#2f4056"],time:3e5,skin:"popup-tips",success:function(i){var r=t(i).position().left-159;t(i).css({left:r});t(".popMenuItem").click(function(){var n=t(this);n.hasClass("layui-nav-itemed")?n.removeClass("layui-nav-itemed"):n.addClass("layui-nav-itemed")});n.initMenuEvent()}})}),r.bind("mouseleave",function(){n.closeMenuTips()}))},closeMenuTips:function(){this.menuTipId!==""&&(f.close(this.menuTipId),this.menuTipId="")}},r={option:null,tabId:"tabMenu",homeTabId:"1",clsTabTitle:".layui-layout-admin .layui-tab-title",clsTabContext:".layui-tab-context",show:function(n){this.option=n;this.initEvent()},loadHome:function(n){n=n||this.option.url.Welcome;var i=t("#ifmWelcome"),r=i.attr("src");r!==n&&i.attr("src",n)},addTab:function(n){var r,u,f,e;n.url&&(r=n.id,u=t(this.clsTabTitle+' li[lay-id="'+r+'"]'),u.length||(f=n.icon+" <span>"+n.text+"<\/span>",e='<iframe src="'+n.url+'" frameborder="0" class="layui-tab-iframe"><\/iframe>',i.tabAdd(this.tabId,{id:r,title:f,content:e})),i.tabChange(this.tabId,r))},getCurTab:function(){var n=t(this.clsTabTitle+" .layui-this"),i=n.attr("lay-id"),r=n.children("span").text(),u=this.option.menus?this.option.menus.find(n=>n.id===i):null;return{id:i,title:r,module:u}},initEvent:function(){var n=this;n.initContextMenu();i.on("tab(tabMenu)",function(){var n=t(this).attr("lay-id");t(".layui-nav-child dd").removeClass("layui-this");t(".layui-nav-child #menu"+n).parent().addClass("layui-this")});t(".layui-tab-left").click(function(){n.roll("left")});t(".layui-tab-right").click(function(){n.roll("right")})},roll:function(n){var i=t(this.clsTabTitle),r=i.scrollLeft();n==="left"?i.animate({scrollLeft:r-450},200):i.animate({scrollLeft:r+450},200)},initContextMenu:function(){var i=this,n=i.clsTabContext,r;t(this.clsTabTitle).contextmenu(function(i){return r=t(i.target),t(n).show().css({left:i.offsetX+10+"px",top:i.offsetY+10+"px"}),!1}).click(function(){t(n).hide()});t(n+' [tab-close="current"]').click(function(){i.closeTab(r,"current")});t(n+' [tab-close="other"]').click(function(){i.closeTab(r,"other")});t(n+' [tab-close="all"]').click(function(){i.closeTab(r,"all")})},closeTab:function(n,r){function u(n){n&&n!==o&&i.tabDelete(e,n)}var e=this.tabId,o=this.homeTabId,f=t(n).parent().attr("lay-id");switch(r){case"current":u(f);break;case"other":t(this.clsTabTitle+" li").each(function(n,i){var r=t(i).attr("lay-id");r!==f&&u(r)});break;case"all":t(this.clsTabTitle+" li").each(function(n,i){u(t(i).attr("lay-id"))})}t(this.clsTabContext).hide()}},u={option:null,show:function(n){this.option=n;this.initEvent()},refresh:function(){var n=t(".layui-layout-admin .layui-tab-content .layui-show iframe"),i=n.attr("src");n.attr("src",i)},share:function(){},cache:function(){e.post(u.option.url.RefreshCache)},fullScreen:function(){o();t(this).data("type","exitScreen").html('<i class="layui-icon layui-icon-screen-restore"><\/i>')},exitScreen:function(){s();t(this).data("type","fullScreen").html('<i class="layui-icon layui-icon-screen-full"><\/i>')},userInfo:function(){r.addTab({id:"userInfo",text:t(this).text(),icon:t(this).find("i")[0].outerHTML,url:u.option.url.UserInfo})},logout:function(){e.confirm("确定要退出系统？",function(){t.post(u.option.url.SignOut,function(n){f.msg(n.message);window.location="/login"})})},initEvent:function(){var n=this;t(".top-right").on("click",function(){var i=t(this),r=i.data("type");n[r]?n[r].call(this,i):""})}};n("admin",{show:function(n){h(n.pageAnim||!0);u.show({url:n.url});t.get(n.url.GetUserMenus,function(i){r.show({menus:i.menus,url:n.url});c.show({data:i.menus,topChanged:function(n){r.loadHome(n.url)}});n.callback&&n.callback(i);t(".loader").fadeOut()})},addTab:function(n){r.addTab(n)},getCurTab:function(){return r.getCurTab()}})});
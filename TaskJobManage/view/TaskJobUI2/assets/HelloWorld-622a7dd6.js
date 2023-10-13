import{r as _,g as G,h as X,d as h,i as P,e as i,j as J,m as Q,_ as ee,o as te,c as ne,b as S,t as D,w as oe,F as ue,k as ae,f as w,p as re,l as ie}from"./index-c267af4a.js";const T=Object.assign,se=typeof window<"u",V=e=>e!==null&&typeof e=="object",y=e=>e!=null,ce=e=>typeof e=="function",L=e=>typeof e=="number"||/^\d+(\.\d+)?$/.test(e),le=()=>se?/ios|iphone|ipad|ipod/.test(navigator.userAgent.toLowerCase()):!1;function z(e,t){const n=t.split(".");let o=e;return n.forEach(c=>{var r;o=V(o)&&(r=o[c])!=null?r:""}),o}const m=[Number,String],de={type:Boolean,default:!0},p=e=>({type:String,default:e});var fe=typeof window<"u",$,C;function ge(){if(!$&&($=_(0),C=_(0),fe)){const e=()=>{$.value=window.innerWidth,C.value=window.innerHeight};e(),window.addEventListener("resize",e,{passive:!0}),window.addEventListener("orientationchange",e,{passive:!0})}return{width:$,height:C}}le();const pe=e=>e.stopPropagation();function me(e,t){(typeof e.cancelable!="boolean"||e.cancelable)&&e.preventDefault(),t&&pe(e)}ge();function f(e){if(y(e))return L(e)?`${e}px`:String(e)}function ve(e){if(y(e)){if(Array.isArray(e))return{width:f(e[0]),height:f(e[1])};const t=f(e);return{width:t,height:t}}}const Ee=/-(\w)/g,W=e=>e.replace(Ee,(t,n)=>n.toUpperCase()),{hasOwnProperty:be}=Object.prototype;function ye(e,t,n){const o=t[n];y(o)&&(!be.call(e,n)||!V(o)?e[n]=o:e[n]=H(Object(e[n]),o))}function H(e,t){return Object.keys(t).forEach(n=>{ye(e,t,n)}),e}var he={name:"姓名",tel:"电话",save:"保存",clear:"清空",cancel:"取消",confirm:"确认",delete:"删除",loading:"加载中...",noCoupon:"暂无优惠券",nameEmpty:"请填写姓名",addContact:"添加联系人",telInvalid:"请填写正确的电话",vanCalendar:{end:"结束",start:"开始",title:"日期选择",weekdays:["日","一","二","三","四","五","六"],monthTitle:(e,t)=>`${e}年${t}月`,rangePrompt:e=>`最多选择 ${e} 天`},vanCascader:{select:"请选择"},vanPagination:{prev:"上一页",next:"下一页"},vanPullRefresh:{pulling:"下拉即可刷新...",loosing:"释放即可刷新..."},vanSubmitBar:{label:"合计:"},vanCoupon:{unlimited:"无门槛",discount:e=>`${e}折`,condition:e=>`满${e}元可用`},vanCouponCell:{title:"优惠券",count:e=>`${e}张可用`},vanCouponList:{exchange:"兑换",close:"不使用",enable:"可用",disabled:"不可用",placeholder:"输入优惠码"},vanAddressEdit:{area:"地区",areaEmpty:"请选择地区",addressEmpty:"请填写详细地址",addressDetail:"详细地址",defaultAddress:"设为默认收货地址"},vanAddressList:{add:"新增地址"}};const O=_("zh-CN"),j=G({"zh-CN":he}),Be={messages(){return j[O.value]},use(e,t){O.value=e,this.add({[e]:t})},add(e={}){H(j,e)}};var $e=Be;function _e(e){const t=W(e)+".";return(n,...o)=>{const c=$e.messages(),r=z(c,t+n)||z(c,n);return ce(r)?r(...o):r}}function F(e,t){return t?typeof t=="string"?` ${e}--${t}`:Array.isArray(t)?t.reduce((n,o)=>n+F(e,o),""):Object.keys(t).reduce((n,o)=>n+(t[o]?F(e,o):""),""):""}function Se(e){return(t,n)=>(t&&typeof t!="string"&&(n=t,t=""),t=t?`${e}__${t}`:e,`${t}${F(t,n)}`)}function B(e){const t=`van-${e}`;return[t,Se(t),_e(t)]}const xe="van-hairline",De=`${xe}--surround`;function x(e){return e.install=t=>{const{name:n}=e;n&&(t.component(n,e),t.component(W(`-${n}`),e))},e}const Ce={to:[String,Object],url:String,replace:Boolean};function we({to:e,url:t,replace:n,$router:o}){e&&o?o[n?"replace":"push"](e):t&&(n?location.replace(t):location.href=t)}function Fe(){const e=X().proxy;return()=>we(e)}const[Pe,N]=B("badge"),Ae={dot:Boolean,max:m,tag:p("div"),color:String,offset:Array,content:m,showZero:de,position:p("top-right")};var Ie=h({name:Pe,props:Ae,setup(e,{slots:t}){const n=()=>{if(t.content)return!0;const{content:a,showZero:s}=e;return y(a)&&a!==""&&(s||a!==0&&a!=="0")},o=()=>{const{dot:a,max:s,content:u}=e;if(!a&&n())return t.content?t.content():y(s)&&L(u)&&+u>+s?`${s}+`:u},c=a=>a.startsWith("-")?a.replace("-",""):`-${a}`,r=P(()=>{const a={background:e.color};if(e.offset){const[s,u]=e.offset,{position:g}=e,[l,v]=g.split("-");t.default?(typeof u=="number"?a[l]=f(l==="top"?u:-u):a[l]=l==="top"?f(u):c(u),typeof s=="number"?a[v]=f(v==="left"?s:-s):a[v]=v==="left"?f(s):c(s)):(a.marginTop=f(u),a.marginLeft=f(s))}return a}),d=()=>{if(n()||e.dot)return i("div",{class:N([e.position,{dot:e.dot,fixed:!!t.default}]),style:r.value},[o()])};return()=>{if(t.default){const{tag:a}=e;return i(a,{class:N("wrapper")},{default:()=>[t.default(),d()]})}return d()}}});const ke=x(Ie),[ze,ot]=B("config-provider"),Oe=Symbol(ze),[je,R]=B("icon"),Ne=e=>e==null?void 0:e.includes("/"),Re={dot:Boolean,tag:p("i"),name:String,size:m,badge:m,color:String,badgeProps:Object,classPrefix:String};var Te=h({name:je,props:Re,setup(e,{slots:t}){const n=J(Oe,null),o=P(()=>e.classPrefix||(n==null?void 0:n.iconPrefix)||R());return()=>{const{tag:c,dot:r,name:d,size:a,badge:s,color:u}=e,g=Ne(d);return i(ke,Q({dot:r,tag:c,class:[o.value,g?"":`${o.value}-${d}`],style:{color:u,fontSize:f(a)},content:s},e.badgeProps),{default:()=>{var l;return[(l=t.default)==null?void 0:l.call(t),g&&i("img",{class:R("image"),src:d},null)]}})}}});const Ve=x(Te),[Le,b]=B("loading"),We=Array(12).fill(null).map((e,t)=>i("i",{class:b("line",String(t+1))},null)),He=i("svg",{class:b("circular"),viewBox:"25 25 50 50"},[i("circle",{cx:"50",cy:"50",r:"20",fill:"none"},null)]),Ue={size:m,type:p("circular"),color:String,vertical:Boolean,textSize:m,textColor:String};var qe=h({name:Le,props:Ue,setup(e,{slots:t}){const n=P(()=>T({color:e.color},ve(e.size))),o=()=>{const r=e.type==="spinner"?We:He;return i("span",{class:b("spinner",e.type),style:n.value},[t.icon?t.icon():r])},c=()=>{var r;if(t.default)return i("span",{class:b("text"),style:{fontSize:f(e.textSize),color:(r=e.textColor)!=null?r:e.color}},[t.default()])};return()=>{const{type:r,vertical:d}=e;return i("div",{class:b([r,{vertical:d}]),"aria-live":"polite","aria-busy":!0},[o(),c()])}}});const Me=x(qe),[Ke,E]=B("button"),Ye=T({},Ce,{tag:p("button"),text:String,icon:String,type:p("default"),size:p("normal"),color:String,block:Boolean,plain:Boolean,round:Boolean,square:Boolean,loading:Boolean,hairline:Boolean,disabled:Boolean,iconPrefix:String,nativeType:p("button"),loadingSize:m,loadingText:String,loadingType:String,iconPosition:p("left")});var Ze=h({name:Ke,props:Ye,emits:["click"],setup(e,{emit:t,slots:n}){const o=Fe(),c=()=>n.loading?n.loading():i(Me,{size:e.loadingSize,type:e.loadingType,class:E("loading")},null),r=()=>{if(e.loading)return c();if(n.icon)return i("div",{class:E("icon")},[n.icon()]);if(e.icon)return i(Ve,{name:e.icon,class:E("icon"),classPrefix:e.iconPrefix},null)},d=()=>{let u;if(e.loading?u=e.loadingText:u=n.default?n.default():e.text,u)return i("span",{class:E("text")},[u])},a=()=>{const{color:u,plain:g}=e;if(u){const l={color:g?u:"white"};return g||(l.background=u),u.includes("gradient")?l.border=0:l.borderColor=u,l}},s=u=>{e.loading?me(u):e.disabled||(t("click",u),o())};return()=>{const{tag:u,type:g,size:l,block:v,round:U,plain:q,square:M,loading:K,disabled:A,hairline:I,nativeType:Y,iconPosition:k}=e,Z=[E([g,l,{plain:q,block:v,round:U,square:M,loading:K,disabled:A,hairline:I}]),{[De]:I}];return i(u,{type:Y,class:Z,style:a(),disabled:A,onClick:s},{default:()=>[i("div",{class:E("content")},[k==="left"&&r(),d(),k==="right"&&r()])]})}}});const Ge=x(Ze);const Xe=h({name:"HelloWorld",props:{msg:{type:String,required:!0}},setup:()=>({count:_(0)})});const Je=e=>(re("data-v-0927e598"),e=e(),ie(),e),Qe=ae('<p data-v-0927e598> Recommended IDE setup: <a href="https://code.visualstudio.com/" target="_blank" data-v-0927e598>VSCode</a> + <a href="https://marketplace.visualstudio.com/items?itemName=octref.vetur" target="_blank" data-v-0927e598> Vetur </a> or <a href="https://github.com/johnsoncodehk/volar" target="_blank" data-v-0927e598>Volar</a> (if using <code data-v-0927e598>&lt;script setup&gt;</code> ) </p><p data-v-0927e598> See <code data-v-0927e598>README.md</code> for more information. </p><p data-v-0927e598><a href="https://vitejs.dev/guide/features.html" target="_blank" data-v-0927e598> Vite Docs </a> | <a href="https://v3.vuejs.org/" target="_blank" data-v-0927e598>Vue 3 Docs</a></p>',3),et=Je(()=>S("p",null,[w(" Edit "),S("code",null,"components/HelloWorld.vue"),w(" to test hot module replacement. ")],-1));function tt(e,t,n,o,c,r){const d=Ge;return te(),ne(ue,null,[S("h1",null,D(e.msg),1),Qe,S("button",{type:"button",onClick:t[0]||(t[0]=a=>e.count++)},"count is: "+D(e.count),1),et,i(d,{type:"primary",round:"",onClick:t[1]||(t[1]=a=>e.count++)},{default:oe(()=>[w(" count is: "+D(e.count),1)]),_:1})],64)}const ut=ee(Xe,[["render",tt],["__scopeId","data-v-0927e598"]]);export{ut as default};

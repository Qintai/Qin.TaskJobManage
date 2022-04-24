<template>
  <div id="app">
    <!-- <img src="./assets/logo.png"> -->
     <router-view/>
  </div>
  </template>
 <script>
export default {
  name: 'App',
   watch: {
        $route(newVal) {
            // console.log(newVal.path); 
        }
    }, 
    mounted: function() {
        this.$router.push('table');
   }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>

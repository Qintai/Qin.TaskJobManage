<template>
  <!-- <img alt="Vue logo" src="./common/assets/logo.png" /> -->
  <!-- <HelloWorld msg="Hello Vue 3 + TypeScript + Vite" /> -->
  <!-- <AA msg="ipt + Vite" /> -->

  <router-view/>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useRouter} from 'vue-router';

export default defineComponent({
  name: 'App',
  components: {
    
  },
  mounted: function() {
    useRouter().push('jobtable');
  }
});
</script>

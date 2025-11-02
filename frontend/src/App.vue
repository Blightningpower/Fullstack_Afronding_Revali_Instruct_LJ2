<template>
  <div id="app">
    <nav>
      <h1>Revali Instruct</h1>
      <router-link to="/patients">Patients</router-link>
    </nav>
    <main style="max-width:980px;margin:1rem auto;padding:0 1rem;">
      <router-view />
    </main>
  </div>
</template>

<script>
export default {}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

nav {
  padding: 1rem;
  background: #2c3e50;
  color: white;
  display: flex;
  align-items: center;
  gap: 2rem;
}

nav a {
  color: white;
  text-decoration: none;
}

main {
  padding: 2rem;
}
</style>
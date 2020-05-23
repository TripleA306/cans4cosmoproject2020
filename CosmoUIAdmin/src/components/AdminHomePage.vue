<template>
    <b-container>
        <b-jumbotron header="Dashboard" class="text-center py-4 mt-2 shadow"></b-jumbotron>
        <div>
            <create-new-admin-modal></create-new-admin-modal>
            <b-button @click="openCreateAdmin">Create new Admin</b-button>
        </div>

    </b-container>
</template>

<script>
    import CreateNewAdminModal from './CreateNewAdminModal.vue'

    export default {
        name: "AdminHomePage",
        components: {
            CreateNewAdminModal
        },
        props:{

        },
        methods: {
            openCreateAdmin: function () {
                this.$bvModal.show('CreateNewAdminModal');
            },
        },
        mounted() {
            if(!sessionStorage.getItem("sessionID"))
            {
                this.$router.push('login');
            }
            else
            {
                this.$emit("login");
            }
        }

    }
</script>

<style scoped>

</style>

<template>
    <b-modal :id="modalId" title="Delete Route"
             @ok="deleteRoute"
             ok-variant="outline-danger"
             cancel-variant="outline-primary" no-close-on-backdrop no-close-on-esc>
            <p>Are you sure you want to delete <span>{{routeName}}</span>?</p>
    </b-modal>
</template>

<script>
    export default {
        name: "AdminRouteDeleteModal",
        props:{
            routeName:{
                type: String,
                default: "Route"
            },
            modalId:{
                type: String,
                default: "delete-modal"
            }
        },
        methods:{
            deleteRoute:function()
            {
                this.$bvModal.hide(this.modalId);
                this.$emit('delete-route')

            }
        },
        computed:
            {
            }
    }
</script>

<style scoped>

</style>
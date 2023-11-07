// Call this to register your module to main application
var moduleName = 'SharePoint';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            $stateProvider
                .state('workspace.SharePointState', {
                    url: '/SharePoint',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        'platformWebApp.bladeNavigationService',
                        function (bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'SharePoint.helloWorldController',
                                template: 'Modules/$(P5.SharePoint)/Scripts/blades/hello-world.html',
                                isClosingDisabled: true,
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['platformWebApp.mainMenuService', '$state',
        function (mainMenuService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/SharePoint',
                icon: 'fa fa-cube',
                title: 'SharePoint',
                priority: 100,
                action: function () { $state.go('workspace.SharePointState'); },
                permission: 'SharePoint:access',
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);

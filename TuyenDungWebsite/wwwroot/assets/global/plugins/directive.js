
angular.module("ui.checkbox", []).directive("checkbox", function() {
	return {
		scope: {},
		require: "ngModel",
		restrict: "E",
		replace: "true",
		template: "<button type=\"button\" ng-style=\"stylebtn\" class=\"btn btn-default\" ng-class=\"{'btn-xs': size==='default', 'btn-sm': size==='large', 'btn-lg': size==='largest', 'checked': checked===true}\">" +
			"<span ng-style=\"styleicon\" class=\"glyphicon\" ng-class=\"{'glyphicon-ok': checked===true, 'glyphicon-minus': checked===undefined}\"></span>" +
			"</button>",
		compile: function compile(elem, attrs, transclude) {
			if(attrs.ngClass !== undefined) {
				attrs.ngClass = attrs.ngClass.replace(/}\s*{/g, ", ");
			}

			return function(scope, elem, attrs, modelCtrl) {
				scope.size = "default";
				// Default Button Styling
				scope.stylebtn = {};
				// Default Checkmark Styling
				scope.styleicon = {"width": "10px", "left": "-1px"};
				// If size is undefined, Checkbox has normal size (Bootstrap 'xs')
				if(attrs.large !== undefined) {
					scope.size = "large";
					scope.stylebtn = {"padding-top": "2px", "padding-bottom": "2px", "height": "30px"};
					scope.styleicon = {"width": "8px", "left": "-5px", "font-size": "17px"};
				}
				if(attrs.larger !== undefined) {
					scope.size = "larger";
					scope.stylebtn = {"padding-top": "2px", "padding-bottom": "2px", "height": "34px"};
					scope.styleicon = {"width": "8px", "left": "-8px", "font-size": "22px"};
				}
				if(attrs.largest !== undefined) {
					scope.size = "largest";
					scope.stylebtn = {"padding-top": "2px", "padding-bottom": "2px", "height": "45px"};
					scope.styleicon = {"width": "11px", "left": "-11px", "font-size": "30px"};
				}
				var indeterminate = false;
				if(attrs.indeterminate === "true") {
					indeterminate = true;
				}

				var trueValue = true;
				var falseValue = false;
				var indeterminateValue = undefined;

				// If defined set true value
				if(attrs.ngTrueValue !== undefined) {
					trueValue = attrs.ngTrueValue;
				}
				// If defined set false value
				if(attrs.ngFalseValue !== undefined) {
					falseValue = attrs.ngFalseValue;
				}
				// If defined set indeterminate value
				if(attrs.ngIndeterminateValue !== undefined) {
					indeterminateValue = attrs.ngIndeterminateValue;
				}

				// Check if name attribute is set and if so add it to the DOM element
				if(scope.name !== undefined) {
					elem.name = scope.name;
				}

				// Update element when model changes
				scope.$watch(function() {
					if(modelCtrl.$modelValue === trueValue || modelCtrl.$modelValue === true) {
						modelCtrl.$setViewValue(trueValue);
					} else if(indeterminate === true && (modelCtrl.$modelValue === indeterminateValue || modelCtrl.$modelValue === undefined)) {
						modelCtrl.$setViewValue(indeterminateValue);
					} else {
						modelCtrl.$setViewValue(falseValue);
					}
					return modelCtrl.$modelValue;
				}, function(newVal, oldVal) {
					if(indeterminate === true && modelCtrl.$modelValue === indeterminateValue) {
						scope.checked = undefined;
					} else {
						scope.checked = modelCtrl.$modelValue === trueValue;
					}
				}, true);

				// On click swap value and trigger onChange function
				elem.bind("click", function() {
					scope.$apply(function() {
						if(indeterminate === true) {
							if(modelCtrl.$modelValue === falseValue) {
								modelCtrl.$setViewValue(trueValue);
							} else if(modelCtrl.$modelValue === trueValue) {
								modelCtrl.$setViewValue(indeterminateValue);
							} else {
								modelCtrl.$setViewValue(falseValue);
							}
						} else {
							if(modelCtrl.$modelValue === falseValue) {
								modelCtrl.$setViewValue(trueValue);
							} else {
								modelCtrl.$setViewValue(falseValue);
							}
						}
					});
				});
			};
		}
	};
});


angular.module('ui.bootstrap.contextMenu', [])
.service('CustomService', function () {
    "use strict";

    return {
        initialize: function (item) {
            //console.log("got here", item);
        }
    }

}).directive('contextMenu', ["$parse", "$q", "CustomService", "$sce", function ($parse, $q, custom, $sce) {

    var contextMenus = [];
    var $currentContextMenu = null;
    var defaultItemText = "New Item";

    var removeContextMenus = function (level) {
        /// <summary>Remove context menu.</summary>
        while (contextMenus.length && (!level || contextMenus.length > level)) {
            contextMenus.pop().remove();
        }
        if (contextMenus.length == 0 && $currentContextMenu) {
            $currentContextMenu.remove();
        }
    };


    var processTextItem = function ($scope, item, text, event, model, $promises, nestedMenu, $) {
        "use strict";

        var $a = $('<a>');
        $a.css("padding-right", "8px");
        $a.attr({ tabindex: '-1', href: '#' });

        if (typeof item[0] === 'string') {
            text = item[0];
        }
        else if (typeof item[0] === "function") {
            text = item[0].call($scope, $scope, event, model);
        } else if (typeof item.text !== "undefined") {
            text = item.text;
        }

        var $promise = $q.when(text);
        $promises.push($promise);
        $promise.then(function (text) {
            if (nestedMenu) {
                $a.css("cursor", "default");
                $a.append($('<strong style="font-family:monospace;font-weight:bold;float:right;">&gt;</strong>'));
            }
            $a.append(text);
        });

        return $a;

    };

    var processItem = function ($scope, event, model, item, $ul, $li, $promises, $q, $, level) {
        /// <summary>Process individual item</summary>
        "use strict";
        // nestedMenu is either an Array or a Promise that will return that array.
        var nestedMenu = angular.isArray(item[1]) || (item[1] && angular.isFunction(item[1].then))
          ? item[1] : angular.isArray(item[2]) || (item[2] && angular.isFunction(item[2].then))
          ? item[2] : angular.isArray(item[3]) || (item[3] && angular.isFunction(item[3].then))
          ? item[3] : null;

        // if html property is not defined, fallback to text, otherwise use default text
        // if first item in the item array is a function then invoke .call()
        // if first item is a string, then text should be the string.

        var text = defaultItemText;
        if (typeof item[0] === 'function' || typeof item[0] === 'string' || typeof item.text !== "undefined") {
            text = processTextItem($scope, item, text, event, model, $promises, nestedMenu, $);
        }
        else if (typeof item.html !== "undefined") {
            // leave styling open to dev
            text = item.html
        }

        $li.append(text);




        // if item is object, and has enabled prop invoke the prop
        // els if fallback to item[2]

        var isEnabled = function () {
            if (typeof item.enabled !== "undefined") {
                return item.enabled.call($scope, $scope, event, model, text);
            } else if (typeof item[2] === "function") {
                return item[2].call($scope, $scope, event, model, text);
            } else {
                return true;
            }
        };

        registerEnabledEvents($scope, isEnabled(), item, $ul, $li, nestedMenu, model, text, event, $, level);
    };

    var handlePromises = function ($ul, level, event, $promises) {
        /// <summary>
        /// calculate if drop down menu would go out of screen at left or bottom
        /// calculation need to be done after element has been added (and all texts are set; thus thepromises)
        /// to the DOM the get the actual height
        /// </summary>
        "use strict";
        $q.all($promises).then(function () {
            var topCoordinate = event.pageY;
            var menuHeight = angular.element($ul[0]).prop('offsetHeight');
            var winHeight = event.view.innerHeight;
            if (topCoordinate > menuHeight && winHeight - topCoordinate < menuHeight) {
                topCoordinate = event.pageY - menuHeight;
            } else if(winHeight <= menuHeight) {
                // If it really can't fit, reset the height of the menu to one that will fit
                angular.element($ul[0]).css({"height": winHeight - 5, "overflow-y": "scroll"});
                // ...then set the topCoordinate height to 0 so the menu starts from the top
                topCoordinate = 0;
            } else if(winHeight - topCoordinate < menuHeight) {
                var reduceThreshold = 5;
                if(topCoordinate < reduceThreshold) {
                    reduceThreshold = topCoordinate;
                }
                topCoordinate = winHeight - menuHeight - reduceThreshold;
            }

            var leftCoordinate = event.pageX;
            var menuWidth = angular.element($ul[0]).prop('offsetWidth');
            var winWidth = event.view.innerWidth;
            var rightPadding = 5;
            if (leftCoordinate > menuWidth && winWidth - leftCoordinate - rightPadding < menuWidth) {
                leftCoordinate = winWidth - menuWidth - rightPadding;
            } else if(winWidth - leftCoordinate < menuWidth) {
                var reduceThreshold = 5;
                if(leftCoordinate < reduceThreshold + rightPadding) {
                    reduceThreshold = leftCoordinate + rightPadding;
                }
                leftCoordinate = winWidth - menuWidth - reduceThreshold - rightPadding;
            }

            $ul.css({
                display: 'block',
                position: 'absolute',
                left: leftCoordinate + 'px',
                top: topCoordinate + 'px'
            });
        });

    };

    var registerEnabledEvents = function ($scope, enabled, item, $ul, $li, nestedMenu, model, text, event, $, level) {
        /// <summary>If item is enabled, register various mouse events.</summary>
        if (enabled) {
            var openNestedMenu = function ($event) {
                removeContextMenus(level + 1);
                /*
                 * The object here needs to be constructed and filled with data
                 * on an "as needed" basis. Copying the data from event directly
                 * or cloning the event results in unpredictable behavior.
                 */
                var ev = {
                    pageX: event.pageX + $ul[0].offsetWidth - 1,
                    pageY: $ul[0].offsetTop + $li[0].offsetTop - 3,
                    view: event.view || window
                };

                /*
                 * At this point, nestedMenu can only either be an Array or a promise.
                 * Regardless, passing them to when makes the implementation singular.
                 */
                $q.when(nestedMenu).then(function(promisedNestedMenu) {
                    renderContextMenu($scope, ev, promisedNestedMenu, model, level + 1);
                });
            };

            $li.on('click', function ($event) {
                $event.preventDefault();
                $scope.$apply(function () {
                    if (nestedMenu) {
                        openNestedMenu($event);
                    } else {
                        $(event.currentTarget).removeClass('context');
                        removeContextMenus();

                        if (angular.isFunction(item[1])) {
                            item[1].call($scope, $scope, event, model, text)
                        } else {
                            item.click.call($scope, $scope, event, model, text);
                        }
                    }
                });
            });

            $li.on('mouseover', function ($event) {
                $scope.$apply(function () {
                    if (nestedMenu) {
                        openNestedMenu($event);
                    }
                });
            });
        } else {
            $li.on('click', function ($event) {
                $event.preventDefault();
            });
            $li.addClass('disabled');
        }

    };


    var renderContextMenu = function ($scope, event, options, model, level, customClass) {
        /// <summary>Render context menu recursively.</summary>
        if (!level) { level = 0; }
        if (!$) { var $ = angular.element; }
        $(event.currentTarget).addClass('context');
        var $contextMenu = $('<div>');
        if ($currentContextMenu) {
            $contextMenu = $currentContextMenu;
        } else {
            $currentContextMenu = $contextMenu;
            $contextMenu.addClass('angular-bootstrap-contextmenu dropdown clearfix');
        }
        if (customClass) {
            $contextMenu.addClass(customClass);
        }
        var $ul = $('<ul>');
        $ul.addClass('dropdown-menu');
        $ul.attr({ 'role': 'menu' });
        $ul.css({
            display: 'block',
            position: 'absolute',
            left: event.pageX + 'px',
            top: event.pageY + 'px',
            "z-index": 10000
        });

        var $promises = [];

        angular.forEach(options, function (item) {

            var $li = $('<li>');
            if (item === null) {
                $li.addClass('divider');
            } else if (typeof item[0] === "object") {
                custom.initialize($li, item);
            } else {
                processItem($scope, event, model, item, $ul, $li, $promises, $q, $, level);
            }
            $ul.append($li);
        });
        $contextMenu.append($ul);
        var height = Math.max(
            document.body.scrollHeight, document.documentElement.scrollHeight,
            document.body.offsetHeight, document.documentElement.offsetHeight,
            document.body.clientHeight, document.documentElement.clientHeight
        );
        $contextMenu.css({
            width: '100%',
            height: height + 'px',
            position: 'absolute',
            top: 0,
            left: 0,
            zIndex: 9999,
            "max-height" : window.innerHeight - 3,
        });
        $(document).find('body').append($contextMenu);

        handlePromises($ul, level, event, $promises);

        $contextMenu.on("mousedown", function (e) {
            if ($(e.target).hasClass('dropdown')) {
                $(event.currentTarget).removeClass('context');
                removeContextMenus();
            }
        }).on('contextmenu', function (event) {
            $(event.currentTarget).removeClass('context');
            event.preventDefault();
            removeContextMenus(level);
        });

        $scope.$on("$destroy", function () {
            removeContextMenus();
        });

        contextMenus.push($ul);
    };

    function isTouchDevice() {
      return 'ontouchstart' in window        // works on most browsers
          || navigator.maxTouchPoints;       // works on IE10/11 and Surface
    };

    return function ($scope, element, attrs) {
        var openMenuEvent = "contextmenu";
        if(attrs.contextMenuOn && typeof(attrs.contextMenuOn) === "string"){
            openMenuEvent = attrs.contextMenuOn;
        }
        element.on(openMenuEvent, function (event) {
            event.stopPropagation();
            event.preventDefault();
            
            // Don't show context menu if on touch device and element is draggable
            if(isTouchDevice() && element.attr('draggable') === 'true') {
              return false;
            }

            $scope.$apply(function () {
                var options = $scope.$eval(attrs.contextMenu);
                var customClass = attrs.contextMenuClass;
                var model = $scope.$eval(attrs.model);
                if (options instanceof Array) {
                    if (options.length === 0) { return; }
                    renderContextMenu($scope, event, options, model, undefined, customClass);
                } else {
                    throw '"' + attrs.contextMenu + '" not an array';
                }
            });
        });
    };
}]); 

angular.module('ui.sortable', [])
 .value('uiSortableConfig', { 
     items: '> [ng-repeat],> [data-ng-repeat],> [x-ng-repeat]'
 })
 .directive('uiSortable', [
   'uiSortableConfig', '$timeout', '$log',
   function (uiSortableConfig, $timeout, $log) {
       return {
           require: '?ngModel',
           scope: {
               ngModel: '=',
               uiSortable: '='
           },
           link: function (scope, element, attrs, ngModel) {
               var savedNodes;

               function combineCallbacks(first, second) {
                   var firstIsFunc = first && (typeof first === 'function');
                   var secondIsFunc = second && (typeof second === 'function');
                   if (firstIsFunc && secondIsFunc) {
                       return function () {
                           first.apply(this, arguments);
                           second.apply(this, arguments);
                       };
                   } else if (secondIsFunc) {
                       return second;
                   }
                   return first;
               }

               function getSortableWidgetInstance(element) {
                   // this is a fix to support jquery-ui prior to v1.11.x
                   // otherwise we should be using `element.sortable('instance')`
                   var data = element.data('ui-sortable');
                   if (data && typeof data === 'object' && data.widgetFullName === 'ui-sortable') {
                       return data;
                   }
                   return null;
               }

               function patchSortableOption(key, value) {
                   if (callbacks[key]) {
                       if (key === 'stop') {
                           // call apply after stop
                           value = combineCallbacks(
                             value, function () { scope.$apply(); });

                           value = combineCallbacks(value, afterStop);
                       }
                       // wrap the callback
                       value = combineCallbacks(callbacks[key], value);
                   } else if (wrappers[key]) {
                       value = wrappers[key](value);
                   }

                   // patch the options that need to have values set
                   if (!value) {
                       if (key === 'items') {
                           value = uiSortableConfig.items;
                       } else if (key === 'ui-model-items') {
                           value = uiSortableConfig.items;
                       }
                   }

                   return value;
               }

               function patchUISortableOptions(newVal, oldVal, sortableWidgetInstance) {
                   function addDummyOptionKey(value, key) {
                       if (!(key in opts)) {
                           // add the key in the opts object so that
                           // the patch function detects and handles it
                           opts[key] = null;
                       }
                   }
                   // for this directive to work we have to attach some callbacks
                   angular.forEach(callbacks, addDummyOptionKey);

                   // only initialize it in case we have to
                   // update some options of the sortable
                   var optsDiff = null;

                   if (oldVal) {
                       // reset deleted options to default
                       var defaultOptions;
                       angular.forEach(oldVal, function (oldValue, key) {
                           if (!newVal || !(key in newVal)) {
                               if (key in directiveOpts) {
                                   if (key === 'ui-floating') {
                                       opts[key] = 'auto';
                                   } else {
                                       opts[key] = patchSortableOption(key, undefined);
                                   }
                                   return;
                               }

                               if (!defaultOptions) {
                                   defaultOptions = angular.element.ui.sortable().options;
                               }
                               var defaultValue = defaultOptions[key];
                               defaultValue = patchSortableOption(key, defaultValue);

                               if (!optsDiff) {
                                   optsDiff = {};
                               }
                               optsDiff[key] = defaultValue;
                               opts[key] = defaultValue;
                           }
                       });
                   }

                   // update changed options
                   angular.forEach(newVal, function (value, key) {
                       // if it's a custom option of the directive,
                       // handle it approprietly
                       if (key in directiveOpts) {
                           if (key === 'ui-floating' && (value === false || value === true) && sortableWidgetInstance) {
                               sortableWidgetInstance.floating = value;
                           }

                           opts[key] = patchSortableOption(key, value);
                           return;
                       }

                       value = patchSortableOption(key, value);

                       if (!optsDiff) {
                           optsDiff = {};
                       }
                       optsDiff[key] = value;
                       opts[key] = value;
                   });

                   return optsDiff;
               }

               function getPlaceholderElement(element) {
                   var placeholder = element.sortable('option', 'placeholder');

                   // placeholder.element will be a function if the placeholder, has
                   // been created (placeholder will be an object).  If it hasn't
                   // been created, either placeholder will be false if no
                   // placeholder class was given or placeholder.element will be
                   // undefined if a class was given (placeholder will be a string)
                   if (placeholder && placeholder.element && typeof placeholder.element === 'function') {
                       var result = placeholder.element();
                       // workaround for jquery ui 1.9.x,
                       // not returning jquery collection
                       result = angular.element(result);
                       return result;
                   }
                   return null;
               }

               function getPlaceholderExcludesludes(element, placeholder) {
                   // exact match with the placeholder's class attribute to handle
                   // the case that multiple connected sortables exist and
                   // the placeholder option equals the class of sortable items
                   var notCssSelector = opts['ui-model-items'].replace(/[^,]*>/g, '');
                   var excludes = element.find('[class="' + placeholder.attr('class') + '"]:not(' + notCssSelector + ')');
                   return excludes;
               }

               function hasSortingHelper(element, ui) {
                   var helperOption = element.sortable('option', 'helper');
                   return helperOption === 'clone' || (typeof helperOption === 'function' && ui.item.sortable.isCustomHelperUsed());
               }

               function getSortingHelper(element, ui, savedNodes) {
                   var result = null;
                   if (hasSortingHelper(element, ui) &&
                       element.sortable('option', 'appendTo') === 'parent') {
                       // The .ui-sortable-helper element (that's the default class name)
                       // is placed last.
                       result = savedNodes.last();
                   }
                   return result;
               }

               // thanks jquery-ui
               function isFloating(item) {
                   return (/left|right/).test(item.css('float')) || (/inline|table-cell/).test(item.css('display'));
               }

               function getElementScope(elementScopes, element) {
                   var result = null;
                   for (var i = 0; i < elementScopes.length; i++) {
                       var x = elementScopes[i];
                       if (x.element[0] === element[0]) {
                           result = x.scope;
                           break;
                       }
                   }
                   return result;
               }

               function afterStop(e, ui) {
                   ui.item.sortable._destroy();
               }

               // return the index of ui.item among the items
               // we can't just do ui.item.index() because there it might have siblings
               // which are not items
               function getItemIndex(ui) {
                   return ui.item.parent()
                     .find(opts['ui-model-items'])
                     .index(ui.item);
               }

               var opts = {};

               // directive specific options
               var directiveOpts = {
                   'ui-floating': undefined,
                   'ui-model-items': uiSortableConfig.items
               };

               var callbacks = {
                   receive: null,
                   remove: null,
                   start: null,
                   stop: null,
                   update: null
               };

               var wrappers = {
                   helper: null
               };

               angular.extend(opts, directiveOpts, uiSortableConfig, scope.uiSortable);

               if (!angular.element.fn || !angular.element.fn.jquery) {
                   $log.error('ui.sortable: jQuery should be included before AngularJS!');
                   return;
               }

               function wireUp() {
                   // When we add or remove elements, we need the sortable to 'refresh'
                   // so it can find the new/removed elements.
                   scope.$watchCollection('ngModel', function () {
                       // Timeout to let ng-repeat modify the DOM
                       $timeout(function () {
                           // ensure that the jquery-ui-sortable widget instance
                           // is still bound to the directive's element
                           if (!!getSortableWidgetInstance(element)) {
                               element.sortable('refresh');
                           }
                       }, 0, false);
                   });

                   callbacks.start = function (e, ui) {
                       if (opts['ui-floating'] === 'auto') {
                           // since the drag has started, the element will be
                           // absolutely positioned, so we check its siblings
                           var siblings = ui.item.siblings();
                           var sortableWidgetInstance = getSortableWidgetInstance(angular.element(e.target));
                           sortableWidgetInstance.floating = isFloating(siblings);
                       }

                       // Save the starting position of dragged item
                       var index = getItemIndex(ui);
                       ui.item.sortable = {
                           model: ngModel.$modelValue[index],
                           index: index,
                           source: ui.item.parent(),
                           sourceModel: ngModel.$modelValue,
                           cancel: function () {
                               ui.item.sortable._isCanceled = true;
                           },
                           isCanceled: function () {
                               return ui.item.sortable._isCanceled;
                           },
                           isCustomHelperUsed: function () {
                               return !!ui.item.sortable._isCustomHelperUsed;
                           },
                           _isCanceled: false,
                           _isCustomHelperUsed: ui.item.sortable._isCustomHelperUsed,
                           _destroy: function () {
                               angular.forEach(ui.item.sortable, function (value, key) {
                                   ui.item.sortable[key] = undefined;
                               });
                           }
                       };
                   };

                   callbacks.activate = function (e, ui) {
                       // We need to make a copy of the current element's contents so
                       // we can restore it after sortable has messed it up.
                       // This is inside activate (instead of start) in order to save
                       // both lists when dragging between connected lists.
                       savedNodes = element.contents();

                       // If this list has a placeholder (the connected lists won't),
                       // don't inlcude it in saved nodes.
                       var placeholder = getPlaceholderElement(element);
                       if (placeholder && placeholder.length) {
                           var excludes = getPlaceholderExcludesludes(element, placeholder);
                           savedNodes = savedNodes.not(excludes);
                       }

                       // save the directive's scope so that it is accessible from ui.item.sortable
                       var connectedSortables = ui.item.sortable._connectedSortables || [];

                       connectedSortables.push({
                           element: element,
                           scope: scope
                       });

                       ui.item.sortable._connectedSortables = connectedSortables;
                   };

                   callbacks.update = function (e, ui) {
                       // Save current drop position but only if this is not a second
                       // update that happens when moving between lists because then
                       // the value will be overwritten with the old value
                       if (!ui.item.sortable.received) {
                           ui.item.sortable.dropindex = getItemIndex(ui);
                           var droptarget = ui.item.parent();
                           ui.item.sortable.droptarget = droptarget;

                           var droptargetScope = getElementScope(ui.item.sortable._connectedSortables, droptarget);
                           ui.item.sortable.droptargetModel = droptargetScope.ngModel;

                           // Cancel the sort (let ng-repeat do the sort for us)
                           // Don't cancel if this is the received list because it has
                           // already been canceled in the other list, and trying to cancel
                           // here will mess up the DOM.
                           element.sortable('cancel');
                       }

                       // Put the nodes back exactly the way they started (this is very
                       // important because ng-repeat uses comment elements to delineate
                       // the start and stop of repeat sections and sortable doesn't
                       // respect their order (even if we cancel, the order of the
                       // comments are still messed up).
                       var sortingHelper = !ui.item.sortable.received && getSortingHelper(element, ui, savedNodes);
                       if (sortingHelper && sortingHelper.length) {
                           // Restore all the savedNodes except from the sorting helper element.
                           // That way it will be garbage collected.
                           savedNodes = savedNodes.not(sortingHelper);
                       }
                       savedNodes.appendTo(element);

                       // If this is the target connected list then
                       // it's safe to clear the restored nodes since:
                       // update is currently running and
                       // stop is not called for the target list.
                       if (ui.item.sortable.received) {
                           savedNodes = null;
                       }

                       // If received is true (an item was dropped in from another list)
                       // then we add the new item to this list otherwise wait until the
                       // stop event where we will know if it was a sort or item was
                       // moved here from another list
                       if (ui.item.sortable.received && !ui.item.sortable.isCanceled()) {
                           scope.$apply(function () {
                               ngModel.$modelValue.splice(ui.item.sortable.dropindex, 0,
                                                          ui.item.sortable.moved);
                           });
                       }
                   };

                   callbacks.stop = function (e, ui) {
                       // If the received flag hasn't be set on the item, this is a
                       // normal sort, if dropindex is set, the item was moved, so move
                       // the items in the list.
                       if (!ui.item.sortable.received &&
                          ('dropindex' in ui.item.sortable) &&
                          !ui.item.sortable.isCanceled()) {

                           scope.$apply(function () {
                               ngModel.$modelValue.splice(
                                 ui.item.sortable.dropindex, 0,
                                 ngModel.$modelValue.splice(ui.item.sortable.index, 1)[0]);
                           });
                       } else {
                           // if the item was not moved, then restore the elements
                           // so that the ngRepeat's comment are correct.
                           if ((!('dropindex' in ui.item.sortable) || ui.item.sortable.isCanceled()) &&
                               !angular.equals(element.contents(), savedNodes)) {

                               var sortingHelper = getSortingHelper(element, ui, savedNodes);
                               if (sortingHelper && sortingHelper.length) {
                                   // Restore all the savedNodes except from the sorting helper element.
                                   // That way it will be garbage collected.
                                   savedNodes = savedNodes.not(sortingHelper);
                               }
                               savedNodes.appendTo(element);
                           }
                       }

                       // It's now safe to clear the savedNodes
                       // since stop is the last callback.
                       savedNodes = null;
                   };

                   callbacks.receive = function (e, ui) {
                       // An item was dropped here from another list, set a flag on the
                       // item.
                       ui.item.sortable.received = true;
                   };

                   callbacks.remove = function (e, ui) {
                       // Workaround for a problem observed in nested connected lists.
                       // There should be an 'update' event before 'remove' when moving
                       // elements. If the event did not fire, cancel sorting.
                       if (!('dropindex' in ui.item.sortable)) {
                           element.sortable('cancel');
                           ui.item.sortable.cancel();
                       }

                       // Remove the item from this list's model and copy data into item,
                       // so the next list can retrive it
                       if (!ui.item.sortable.isCanceled()) {
                           scope.$apply(function () {
                               ui.item.sortable.moved = ngModel.$modelValue.splice(
                                 ui.item.sortable.index, 1)[0];
                           });
                       }
                   };

                   wrappers.helper = function (inner) {
                       if (inner && typeof inner === 'function') {
                           return function (e, item) {
                               var innerResult = inner.apply(this, arguments);
                               item.sortable._isCustomHelperUsed = item !== innerResult;
                               return innerResult;
                           };
                       }
                       return inner;
                   };

                   scope.$watchCollection('uiSortable', function (newVal, oldVal) {
                       // ensure that the jquery-ui-sortable widget instance
                       // is still bound to the directive's element
                       var sortableWidgetInstance = getSortableWidgetInstance(element);
                       if (!!sortableWidgetInstance) {
                           var optsDiff = patchUISortableOptions(newVal, oldVal, sortableWidgetInstance);

                           if (optsDiff) {
                               element.sortable('option', optsDiff);
                           }
                       }
                   }, true);

                   patchUISortableOptions(opts);
               }

               function init() {
                   if (ngModel) {
                       wireUp();
                   } else {
                       $log.info('ui.sortable: ngModel not provided!', element);
                   }

                   // Create sortable
                   element.sortable(opts);
               }

               function initIfEnabled() {
                   if (scope.uiSortable && scope.uiSortable.disabled) {
                       return false;
                   }

                   init();

                   // Stop Watcher
                   initIfEnabled.cancelWatcher();
                   initIfEnabled.cancelWatcher = angular.noop;

                   return true;
               }

               initIfEnabled.cancelWatcher = angular.noop;

               if (!initIfEnabled()) {
                   initIfEnabled.cancelWatcher = scope.$watch('uiSortable.disabled', initIfEnabled);
               }
           }
       };
   }
 ]);

angular.module('checklist-model', [])
.directive('checklistModel', ['$parse', '$compile', function($parse, $compile) {
  // contains
  function contains(arr, item, comparator) {
    if (angular.isArray(arr)) {
      for (var i = arr.length; i--;) {
        if (comparator(arr[i], item)) {
          return true;
        }
      }
    }
    return false;
  }

  // add
  function add(arr, item, comparator) {
    arr = angular.isArray(arr) ? arr : [];
      if(!contains(arr, item, comparator)) {
          arr.push(item);
      }
    return arr;
  }  

  // remove
  function remove(arr, item, comparator) {
    if (angular.isArray(arr)) {
      for (var i = arr.length; i--;) {
        if (comparator(arr[i], item)) {
          arr.splice(i, 1);
          break;
        }
      }
    }
    return arr;
  }

  // http://stackoverflow.com/a/19228302/1458162
  function postLinkFn(scope, elem, attrs) {
     // exclude recursion, but still keep the model
    var checklistModel = attrs.checklistModel;
    attrs.$set("checklistModel", null);
    // compile with `ng-model` pointing to `checked`
    $compile(elem)(scope);
    attrs.$set("checklistModel", checklistModel);

    // getter / setter for original model
    var getter = $parse(checklistModel);
    var setter = getter.assign;
    var checklistChange = $parse(attrs.checklistChange);
    var checklistBeforeChange = $parse(attrs.checklistBeforeChange);

    // value added to list
    var value = attrs.checklistValue ? $parse(attrs.checklistValue)(scope.$parent) : attrs.value;


    var comparator = angular.equals;

    if (attrs.hasOwnProperty('checklistComparator')){
      if (attrs.checklistComparator[0] == '.') {
        var comparatorExpression = attrs.checklistComparator.substring(1);
        comparator = function (a, b) {
          return a[comparatorExpression] === b[comparatorExpression];
        };
        
      } else {
        comparator = $parse(attrs.checklistComparator)(scope.$parent);
      }
    }

    // watch UI checked change
    scope.$watch(attrs.ngModel, function(newValue, oldValue) {
      if (newValue === oldValue) { 
        return;
      } 

      if (checklistBeforeChange && (checklistBeforeChange(scope) === false)) {
        scope[attrs.ngModel] = contains(getter(scope.$parent), value, comparator);
        return;
      }

      setValueInChecklistModel(value, newValue);

      if (checklistChange) {
        checklistChange(scope);
      }
    });

    function setValueInChecklistModel(value, checked) {
      var current = getter(scope.$parent);
      if (angular.isFunction(setter)) {
        if (checked === true) {
          setter(scope.$parent, add(current, value, comparator));
        } else {
          setter(scope.$parent, remove(current, value, comparator));
        }
      }
      
    }

    // declare one function to be used for both $watch functions
    function setChecked(newArr, oldArr) {
      if (checklistBeforeChange && (checklistBeforeChange(scope) === false)) {
        setValueInChecklistModel(value, scope[attrs.ngModel]);
        return;
      }
      scope[attrs.ngModel] = contains(newArr, value, comparator);
    }

    // watch original model change
    // use the faster $watchCollection method if it's available
    if (angular.isFunction(scope.$parent.$watchCollection)) {
        scope.$parent.$watchCollection(checklistModel, setChecked);
    } else {
        scope.$parent.$watch(checklistModel, setChecked, true);
    }
  }

  return {
    restrict: 'A',
    priority: 1000,
    terminal: true,
    scope: true,
    compile: function(tElement, tAttrs) {
      if ((tElement[0].tagName !== 'INPUT' || tAttrs.type !== 'checkbox') && (tElement[0].tagName !== 'MD-CHECKBOX') && (!tAttrs.btnCheckbox)) {
        throw 'checklist-model should be applied to `input[type="checkbox"]` or `md-checkbox`.';
      }

      if (!tAttrs.checklistValue && !tAttrs.value) {
        throw 'You should provide `value` or `checklist-value`.';
      }

      // by default ngModel is 'checked', so we set it if not specified
      if (!tAttrs.ngModel) {
        // local scope var storing individual checkbox model
        tAttrs.$set("ngModel", "checked");
      }

      return postLinkFn;
    }
  };
}]);


(function (root, factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    define(['angular'], factory);
  } else if (typeof module !== 'undefined' && typeof module.exports === 'object') {
    module.exports = factory(require('angular'));
  } else {
    return factory(root.angular);
  }
}(this, function (angular) {
angular.module('angular-confirm', ['ui.bootstrap.modal'])
  .controller('ConfirmModalController', ["$scope", "$uibModalInstance", "data", function ($scope, $uibModalInstance, data) {
    $scope.data = angular.copy(data);

    $scope.ok = function (closeMessage) {
      $uibModalInstance.close(closeMessage);
    };

    $scope.cancel = function (dismissMessage) {
      if (angular.isUndefined(dismissMessage)) {
        dismissMessage = 'cancel';
      }
      $uibModalInstance.dismiss(dismissMessage);
    };

  }])
  .value('$confirmModalDefaults', {
    template: '<div class="modal-header"><h3 class="modal-title">{{data.title}}</h3></div>' +
    '<div class="modal-body">{{data.text}}</div>' +
    '<div class="modal-footer">' +
    '<button class="btn btn-primary" ng-click="ok()">{{data.ok}}</button>' +
    '<button class="btn btn-default" ng-click="cancel()">{{data.cancel}}</button>' +
    '</div>',
    controller: 'ConfirmModalController',
    defaultLabels: {
      title: 'Confirm',
      ok: 'OK',
      cancel: 'Cancel'
    }
  })
  .factory('$confirm', ["$uibModal", "$confirmModalDefaults", function ($uibModal, $confirmModalDefaults) {
    return function (data, settings) {
      var defaults = angular.copy($confirmModalDefaults);
      settings = angular.extend(defaults, (settings || {}));
      
      data = angular.extend({}, settings.defaultLabels, data || {});

      if ('templateUrl' in settings && 'template' in settings) {
        delete settings.template;
      }

      settings.resolve = {
        data: function () {
          return data;
        }
      };

      return $uibModal.open(settings).result;
    };
  }])
  .directive('confirm', ["$confirm", "$timeout", function ($confirm, $timeout) {
    return {
      priority: 1,
      restrict: 'A',
      scope: {
        confirmIf: "=",
        ngClick: '&',
        confirm: '@',
        confirmSettings: "=",
        confirmTitle: '@',
        confirmOk: '@',
        confirmCancel: '@'
      },
      link: function (scope, element, attrs) {

        function onSuccess() {
          var rawEl = element[0];
          if (["checkbox", "radio"].indexOf(rawEl.type) != -1) {
            var model = element.data('$ngModelController');
            if (model) {
              model.$setViewValue(!rawEl.checked);
              model.$render();
            } else {
              rawEl.checked = !rawEl.checked;
            }
          }
          scope.ngClick();
        }

        element.unbind("click").bind("click", function ($event) {

          $event.preventDefault();

          $timeout(function() {

            if (angular.isUndefined(scope.confirmIf) || scope.confirmIf) {
              var data = {text: scope.confirm};
              if (scope.confirmTitle) {
                data.title = scope.confirmTitle;
              }
              if (scope.confirmOk) {
                data.ok = scope.confirmOk;
              }
              if (scope.confirmCancel) {
                data.cancel = scope.confirmCancel;
              }
              $confirm(data, scope.confirmSettings || {}).then(onSuccess);
            } else {
              scope.$apply(onSuccess);
            }

          });

        });

      }
    }
  }]);
}));


angular.module( "ngAutocomplete", [])
  .directive('ngAutocomplete', function() {
    return {
      require: 'ngModel',
      scope: {
        ngModel: '=',
        options: '=?',
        details: '=?'
      },

      link: function(scope, element, attrs, controller) {

        //options for autocomplete
        var opts
        var watchEnter = false
        //convert options provided to opts
        var initOpts = function() {

          opts = {}
          if (scope.options) {

            if (scope.options.watchEnter !== true) {
              watchEnter = false
            } else {
              watchEnter = true
            }

            if (scope.options.types) {
              opts.types = []
              opts.types.push(scope.options.types)
              scope.gPlace.setTypes(opts.types)
            } else {
              scope.gPlace.setTypes([])
            }

            if (scope.options.bounds) {
              opts.bounds = scope.options.bounds
              scope.gPlace.setBounds(opts.bounds)
            } else {
              scope.gPlace.setBounds(null)
            }

            if (scope.options.country) {
              opts.componentRestrictions = {
                country: scope.options.country
              }
              scope.gPlace.setComponentRestrictions(opts.componentRestrictions)
            } else {
              scope.gPlace.setComponentRestrictions(null)
            }
          }
        }

        if (scope.gPlace == undefined) {
          scope.gPlace = new google.maps.places.Autocomplete(element[0], {});
        }
        google.maps.event.addListener(scope.gPlace, 'place_changed', function() {
          var result = scope.gPlace.getPlace();
          if (result !== undefined) {
            if (result.address_components !== undefined) {

              scope.$apply(function() {

                scope.details = result;

                controller.$setViewValue(element.val());
              });
            }
            else {
              if (watchEnter) {
                getPlace(result)
              }
            }
          }
        })

        //function to get retrieve the autocompletes first result using the AutocompleteService 
        var getPlace = function(result) {
          var autocompleteService = new google.maps.places.AutocompleteService();
          if (result.name.length > 0){
            autocompleteService.getPlacePredictions(
              {
                input: result.name,
                offset: result.name.length
              },
              function listentoresult(list, status) {
                if(list == null || list.length == 0) {

                  scope.$apply(function() {
                    scope.details = null;
                  });

                } else {
                  var placesService = new google.maps.places.PlacesService(element[0]);
                  placesService.getDetails(
                    {'reference': list[0].reference},
                    function detailsresult(detailsResult, placesServiceStatus) {

                      if (placesServiceStatus == google.maps.GeocoderStatus.OK) {
                        scope.$apply(function() {

                          controller.$setViewValue(detailsResult.formatted_address);
                          element.val(detailsResult.formatted_address);

                          scope.details = detailsResult;

                          //on focusout the value reverts, need to set it again.
                          var watchFocusOut = element.on('focusout', function(event) {
                            element.val(detailsResult.formatted_address);
                            element.unbind('focusout')
                          })

                        });
                      }
                    }
                  );
                }
              });
          }
        }

        controller.$render = function () {
          var location = controller.$viewValue;
          element.val(location);
        };

        //watch options provided to directive
        scope.watchOptions = function () {
          return scope.options
        };
        scope.$watch(scope.watchOptions, function () {
          initOpts()
        }, true);

      }
    };
  });



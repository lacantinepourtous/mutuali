import Vue from "vue";
import VueRouter from "vue-router";

import AuthentificationService from "@/services/authentification";
import AppService from "@/services/app";

import { URL_LOGIN, URL_ROOT, URL_LIST_AD } from "@/consts/urls";

import routes from "./routes";

Vue.use(VueRouter);

function scrollBehavior(to, from, savedPosition) {
  // Mimics the behavior of scrolling to an anchor
  if (to.hash) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const elToScroll = document.querySelector(to.hash);
        if (!elToScroll) {
          return { x: 0, y: 0, behavior: "smooth" };
        }
        const yPos = elToScroll.getBoundingClientRect().top;
        resolve({ x: 0, y: yPos, behavior: "smooth" });
      }, 500);
    });
  }

  return savedPosition || { x: 0, y: 0 };
}

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
  scrollBehavior
});

router.beforeEach((to, from, next) => {
  const isLoggedIn = AuthentificationService.getUserIsLogged();
  const userType = AuthentificationService.getUserType();

  if (to.matched.some((r) => !r.meta.anonymous)) {
    if (!isLoggedIn) {
      return next({
        name: URL_LOGIN,
        query: {
          returnPath: to.fullPath
        }
      });
    }
  } else if (to.matched.some((r) => r.meta.notConnected)) {
    {
      if (isLoggedIn) {
        return next({
          name: URL_LIST_AD
        });
      }
    }
  }

  for (const match of to.matched) {
    if (match.meta.usertype) {
      if (Array.isArray(match.meta.usertype)) {
        if (!match.meta.usertype.includes(userType)) {
          return next({
            name: URL_ROOT
          });
        }
      } else if (userType !== match.meta.usertype) {
        return next({
          name: URL_ROOT
        });
      }
    }
  }

  AppService.updateShowMenu(!to.meta.fullscreen);
  next();
});

export default router;

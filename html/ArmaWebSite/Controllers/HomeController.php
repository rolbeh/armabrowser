<?php


Core\Route::AddRoute('/Home', 'Home');

/**
 * Home short summary.
 *
 * Home description.
 *
 * @version 1.0
 * @author Fake
 */
class HomeController extends Core\Controller
{
    public function Index(){

        $this->SetTitle(DEFAULT_TITLE . "Free");

        $this->SetPageDate(date('c', mktime(00, 00, 00, 8, 18, 15)));

        $this->RenderView();

    }
}

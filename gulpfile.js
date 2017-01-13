const _ = require('lodash/fp');
const args = require('yargs').argv;
const gulp = require('gulp');
const gutil = require('gulp-util');
// const log = gutil.log;
const path = require('path');
const proc = require('child_process');
const usage = require('gulp-help-doc');

gulp.task('help', function() { return usage(gulp); });

/*
 TODO: Document this.
 */
gulp.task('test', function testRavenAssure(done) {
   debugger;
   var testProcess = proc
      .spawn('dotnet', ['test'], {
         cwd: path.join('src', 'Raven.Assure.Test')
      });

   // dotnet CLI adds line break, so when combined with log it adds an additional empty line.
   var removeLineBreaks = (msg) => msg.replace(/(\r\n|\r|\n)/g, '');
   var toString = (msg) => msg.toString();
   var logDatar = _.flow(toString, removeLineBreaks, gutil.log);
   var logError = _.flow(gutil.colors.red, logDatar);

   testProcess.stdout.on('data', logDatar);
   testProcess.stderr.on('data', logError);
   testProcess.on('close', done);
});
